using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Mobile.BuildTools.LibSass
{
    /// <summary>
    /// Scss to css converter using libsass from https://github.com/sass/libsass.
    /// </summary>
    public static class Scss
    {
        // NOTE: It is important to keep allocated delegate importer function, so that it will not be garbage collected
        private static readonly LibSass.sass_importer_delegate ScssImporterLock;
        private static readonly IntPtr ScssImporterPtr;
        private static string version;
        private static string languageVersion;
        private static readonly ScssOptions DefaultOptions = new ScssOptions();

        static Scss()
        {
            // We must store the delegate so GetFunctionPointerForDelegate will not be orphaned
            ScssImporterLock = CustomScssImporter;
            ScssImporterPtr = Marshal.GetFunctionPointerForDelegate(ScssImporterLock);
        }

        /// <summary>
        /// Gets the libsass version.
        /// </summary>
        public static string Version => version ??= LibSass.libsass_version();

        /// <summary>
        /// Gets the libsass language version.
        /// </summary>
        public static string LanguageVersion => languageVersion ??= LibSass.libsass_language_version();

        /// <summary>
        /// Converts the specified scss content string.
        /// </summary>
        /// <param name="scss">A scss content.</param>
        /// <param name="options">The options.</param>
        /// <returns>The result of the conversion</returns>
        /// <exception cref="System.ArgumentNullException">if scss is null</exception>
        public static ScssResult ConvertToCss(string scss, ScssOptions options = null)
        {
            if (scss == null) throw new ArgumentNullException(nameof(scss));
            return FromCore(scss, false, options);
        }

        /// <summary>
        /// Converts a scss content from the specified scss file.
        /// </summary>
        /// <param name="scssFile">A scss file.</param>
        /// <param name="options">The options.</param>
        /// <returns>The result of the conversion</returns>
        /// <exception cref="System.ArgumentNullException">if scss is null</exception>
        public static ScssResult ConvertFileToCss(string scssFile, ScssOptions options = null)
        {
            if (scssFile == null) throw new ArgumentNullException(nameof(scssFile));
            return FromCore(scssFile, true, options);
        }

        /// <summary>
        /// Shared conversion method.
        /// </summary>
        /// <param name="fromStringOrFile">From string or file.</param>
        /// <param name="fromFile">if set to <c>true</c> <paramref name="fromStringOrFile"/> is a scss file; otherwise it is a scss content.</param>
        /// <param name="options">The options.</param>
        /// <returns>The result of the conversion</returns>
        private static ScssResult FromCore(string fromStringOrFile, bool fromFile, ScssOptions options = null)
        {
            if (string.IsNullOrEmpty(fromStringOrFile) && (string.IsNullOrEmpty(options?.InputFile) || !File.Exists(options.InputFile)))
            {
                throw new Exception("You must provide the scss file contents or file path in the ScssOptions");
            }

            if (string.IsNullOrEmpty(fromStringOrFile))
            {
                fromStringOrFile = File.ReadAllText(options.InputFile);
            }

            var compiler = new LibSass.Sass_Compiler();
            GCHandle? tryImportHandle = null;
            var context = new LibSass.Sass_Context();
            try
            {
                options ??= DefaultOptions;
                if (fromFile)
                {
                    var fileContext = LibSass.sass_make_file_context(fromStringOrFile);
                    context = fileContext;
                    if (options.InputFile == null)
                    {
                        options.InputFile = fromStringOrFile;
                    }
                    tryImportHandle = MarshalOptions(fileContext, options);
                    compiler = LibSass.sass_make_file_compiler(fileContext);
                }
                else
                {
                    var dataContext = LibSass.sass_make_data_context(fromStringOrFile);
                    context = dataContext;
                    tryImportHandle = MarshalOptions(dataContext, options);
                    compiler = LibSass.sass_make_data_compiler(dataContext);
                }

                LibSass.sass_compiler_parse(compiler);
                CheckStatus(context);

                LibSass.sass_compiler_execute(compiler);
                CheckStatus(context);

                // Gets the result of the conversion
                var css = LibSass.sass_context_get_output_string(context);

                // Gets the map if it was enabled
                string map = null;
                if (options != null && options.GenerateSourceMap)
                {
                    map = LibSass.sass_context_get_source_map_string(context);
                }

                // Gets the list of included files
                var includedFiles = GetIncludedFiles(context);

                // Returns the result
                return new ScssResult(css, map, includedFiles);
            }
            finally
            {
                // Release the cookie handle if any
                if (tryImportHandle.HasValue && tryImportHandle.Value.IsAllocated)
                {
                    tryImportHandle.Value.Free();
                }

                if (compiler.Pointer != IntPtr.Zero)
                {
                    LibSass.sass_delete_compiler(compiler);
                }

                if (context.Pointer != IntPtr.Zero)
                {
                    if (fromFile)
                    {
                        LibSass.sass_delete_file_context((LibSass.Sass_File_Context)context);
                    }
                    else
                    {
                        LibSass.sass_delete_data_context((LibSass.Sass_Data_Context)context);
                    }
                }
            }
        }

        private static unsafe List<string> GetIncludedFiles(LibSass.Sass_Context context)
        {
            var filesCount = (int)LibSass.sass_context_get_included_files_size(context);
            var files = (LibSass.StringUtf8*)LibSass.sass_context_get_included_files(context);
            List<string> list = null;
            for (var i = 0; i < filesCount; i++)
            {
                if (!files->IsEmpty)
                {
                    if (list == null)
                    {
                        list = new List<string>();
                    }
                    list.Add(*files);
                }
                files++;
            }
            return list;
        }

        private static void CheckStatus(LibSass.Sass_Context context)
        {
            var status = LibSass.sass_context_get_error_status(context);
            if (status != 0)
            {
                var column = LibSass.sass_context_get_error_column(context);
                var line = LibSass.sass_context_get_error_line(context);
                var file = (string)LibSass.sass_context_get_error_file(context);
                var message = (string)LibSass.sass_context_get_error_message(context);
                var errorText = (string)LibSass.sass_context_get_error_text(context);

                throw new ScssException((int)line, (int)column, file, message, errorText);
            }
        }

        private static GCHandle? MarshalOptions(LibSass.Sass_Context context, ScssOptions options)
        {
            var nativeOptions = LibSass.sass_context_get_options(context);

            // TODO: The C function is not working?
            //LibSass.sass_option_set_precision(nativeOptions, options.Precision);
            LibSass.sass_option_set_output_style(nativeOptions, (LibSass.Sass_Output_Style)(int)options.OutputStyle);
            LibSass.sass_option_set_source_comments(nativeOptions, options.SourceComments);
            LibSass.sass_option_set_source_map_embed(nativeOptions, options.SourceMapEmbed);
            LibSass.sass_option_set_source_map_contents(nativeOptions, options.SourceMapContents);
            LibSass.sass_option_set_omit_source_map_url(nativeOptions, options.OmitSourceMapUrl);
            LibSass.sass_option_set_is_indented_syntax_src(nativeOptions, options.IsIndentedSyntaxSource);

            // Handle TryImport
            GCHandle? cookieHandle = null;
            if (options.TryImport != null)
            {
                unsafe
                {
                    var importerList = LibSass.sass_make_importer_list(1);
                    cookieHandle = GCHandle.Alloc(options.TryImport, GCHandleType.Normal);
                    var cookie = GCHandle.ToIntPtr(cookieHandle.Value);

                    var importer = LibSass.sass_make_importer(new LibSass.Sass_Importer_Fn(ScssImporterPtr), 0, cookie);
                    LibSass.sass_importer_set_list_entry(importerList, 0, importer);
                    LibSass.sass_option_set_c_importers(nativeOptions, importerList);
                    // TODO: Should we deallocate with sass_delete_importer at some point?
                }
            }

            if (options.Indent != null)
            {
                LibSass.sass_option_set_indent(nativeOptions, options.Indent);
            }
            if (options.Linefeed != null)
            {
                LibSass.sass_option_set_linefeed(nativeOptions, options.Linefeed);
            }
            if (options.InputFile != null)
            {
                var inputFile = GetRootedPath(options.InputFile);
                LibSass.sass_option_set_input_path(nativeOptions, inputFile);
            }
            string outputFile = null;
            if (options.OutputFile != null)
            {
                outputFile = GetRootedPath(options.OutputFile);
                LibSass.sass_option_set_output_path(nativeOptions, outputFile);
            }
            //if (options.PluginPaths.Count > 0)
            //{
            //    foreach (var path in options.PluginPaths)
            //    {
            //        LibSass.sass_option_push_plugin_path(nativeOptions, path);
            //    }
            //}
            if (options.IncludePaths.Count > 0)
            {
                foreach (var path in options.IncludePaths)
                {
                    var fullPath = GetRootedPath(path);
                    LibSass.sass_option_push_include_path(nativeOptions, fullPath);
                }
            }
            if (options.GenerateSourceMap)
            {
                var sourceMapFile = GetRootedPath(options.SourceMapFile ?? (outputFile ?? "result.css") + ".map");
                if (options.SourceMapFile == null)
                {
                    options.SourceMapFile = sourceMapFile;
                }

                LibSass.sass_option_set_source_map_file(nativeOptions, sourceMapFile);
            }
            if (options.SourceMapRoot != null)
            {
                LibSass.sass_option_set_source_map_root(nativeOptions, options.SourceMapRoot);
            }

            return cookieHandle;
        }

        private static string GetRootedPath(string path) =>
            !Path.IsPathRooted(path) ? Path.Combine(Directory.GetCurrentDirectory(), path) : path;

        private static unsafe LibSass.Sass_Import_List CustomScssImporter(LibSass.StringUtf8 currentPath, LibSass.Sass_Importer_Entry cb, LibSass.Sass_Compiler compiler)
        {
            var cookie = LibSass.sass_importer_get_cookie(cb);

            var previous = LibSass.sass_compiler_get_last_import(compiler);
            string previousPath = LibSass.sass_import_get_abs_path(previous);

            var cookieHandle = GCHandle.FromIntPtr(cookie);
            var tryImport = (ScssOptions.TryImportDelegate)cookieHandle.Target;

            var file = (string)currentPath;
            var importList = LibSass.sass_make_import_list(1);
            uint line = 0;
            uint column = 0;
            string errorMessage = null;
            if (tryImport != null)
            {
                try
                {
                    if (tryImport(file, previousPath, out var scss, out var map))
                    {
                        var entry = LibSass.sass_make_import_entry(file, scss, map);
                        *(LibSass.Sass_Import_Entry*)importList.Pointer = entry;
                        return importList;
                    }
                }
                catch (ScssException ex)
                {
                    errorMessage = ex.Message;
                    line = (uint)ex.Line;
                    column = (uint)ex.Column;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.ToString();
                }
            }

            if (errorMessage == null)
            {
                errorMessage = $"Unable to find include file for @import \"{file}\" with dynamic import";
            }
            {
                var entry = LibSass.sass_make_import_entry(file, null, null);
                *(LibSass.Sass_Import_Entry*)importList.Pointer = entry;

                LibSass.sass_import_set_error(entry, errorMessage, line, column);
            }
            return importList;
        }
    }
}
