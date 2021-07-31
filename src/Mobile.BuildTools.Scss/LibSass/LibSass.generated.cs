using System;
using System.Runtime.InteropServices;

namespace Mobile.BuildTools.LibSass
{
    internal static partial class LibSass
    {
        public enum Sass_Output_Style
        {
            SASS_STYLE_NESTED = 0,
            SASS_STYLE_EXPANDED = 1,
            SASS_STYLE_COMPACT = 2,
            SASS_STYLE_COMPRESSED = 3,
            SASS_STYLE_INSPECT = 4,
            SASS_STYLE_TO_SASS = 5,
        }

        /// <summary>
        /// to allocate buffer to be filled
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_alloc_memory", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sass_alloc_memory(size_t size);

        /// <summary>
        /// to allocate a buffer from existing string
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_copy_c_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_copy_c_string(StringUtf8 str);

        /// <summary>
        /// to free overtaken memory when done
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_free_memory", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_free_memory(IntPtr ptr);

        /// <summary>
        /// Some convenient string helper function
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_string_quote", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_string_quote(StringUtf8 str, sbyte quote_mark);

        [DllImport(LibSassDll, EntryPoint = "sass_string_unquote", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_string_unquote(StringUtf8 str);

        /// <summary>
        /// Implemented sass language version Hardcoded version 3.4 for time being
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "libsass_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 libsass_version();

        /// <summary>
        /// Get compiled libsass language
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "libsass_language_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 libsass_language_version();

        public partial struct Sass_Value
        {
            public Sass_Value(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public enum Sass_Tag
        {
            SASS_BOOLEAN = 0,
            SASS_NUMBER = 1,
            SASS_COLOR = 2,
            SASS_STRING = 3,
            SASS_LIST = 4,
            SASS_MAP = 5,
            SASS_NULL = 6,
            SASS_ERROR = 7,
            SASS_WARNING = 8,
        }

        public enum Sass_Separator
        {
            SASS_COMMA = 0,
            SASS_SPACE = 1,
            SASS_HASH = 2,
        }

        public enum Sass_OP
        {
            AND = 0,
            OR = 1,
            EQ = 2,
            NEQ = 3,
            GT = 4,
            GTE = 5,
            LT = 6,
            LTE = 7,
            ADD = 8,
            SUB = 9,
            MUL = 10,
            DIV = 11,
            MOD = 12,
            NUM_OPS = 13,
        }

        /// <summary>
        /// Creator functions for all value types
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_make_null", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_make_null();

        [DllImport(LibSassDll, EntryPoint = "sass_make_boolean", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_make_boolean(bool val);

        [DllImport(LibSassDll, EntryPoint = "sass_make_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_make_string(StringUtf8 val);

        [DllImport(LibSassDll, EntryPoint = "sass_make_qstring", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_make_qstring(StringUtf8 val);

        [DllImport(LibSassDll, EntryPoint = "sass_make_number", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_make_number(double val, StringUtf8 unit);

        [DllImport(LibSassDll, EntryPoint = "sass_make_color", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_make_color(double r, double g, double b, double a);

        [DllImport(LibSassDll, EntryPoint = "sass_make_list", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_make_list(size_t len, Sass_Separator sep, bool is_bracketed);

        [DllImport(LibSassDll, EntryPoint = "sass_make_map", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_make_map(size_t len);

        [DllImport(LibSassDll, EntryPoint = "sass_make_error", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_make_error(StringUtf8 msg);

        [DllImport(LibSassDll, EntryPoint = "sass_make_warning", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_make_warning(StringUtf8 msg);

        /// <summary>
        /// Generic destructor function for all types Will release memory of all associated
        /// Sass_Values Means we will delete recursively for lists and maps
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_delete_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_delete_value(Sass_Value val);

        /// <summary>
        /// Make a deep cloned copy of the given sass value
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_clone_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_clone_value(Sass_Value val);

        /// <summary>
        /// Execute an operation for two Sass_Values and return the result as a Sass_Value
        /// too
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_value_op", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_value_op(Sass_OP op, Sass_Value a, Sass_Value b);

        /// <summary>
        /// Stringify a Sass_Values and also return the result as a Sass_Value (of type
        /// STRING)
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_value_stringify", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_value_stringify(Sass_Value a, bool compressed, int precision);

        /// <summary>
        /// Return the sass tag for a generic sass value Check is needed before accessing
        /// specific values!
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_value_get_tag", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Tag sass_value_get_tag(Sass_Value v);

        /// <summary>
        /// Check value to be of a specific type Can also be used before accessing
        /// properties!
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_value_is_null", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_value_is_null(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_value_is_number", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_value_is_number(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_value_is_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_value_is_string(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_value_is_boolean", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_value_is_boolean(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_value_is_color", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_value_is_color(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_value_is_list", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_value_is_list(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_value_is_map", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_value_is_map(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_value_is_error", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_value_is_error(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_value_is_warning", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_value_is_warning(Sass_Value v);

        /// <summary>
        /// Getters and setters for Sass_Number
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_number_get_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern double sass_number_get_value(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_number_set_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_number_set_value(Sass_Value v, double value);

        [DllImport(LibSassDll, EntryPoint = "sass_number_get_unit", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_number_get_unit(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_number_set_unit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_number_set_unit(Sass_Value v, StringUtf8 unit);

        /// <summary>
        /// Getters and setters for Sass_String
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_string_get_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_string_get_value(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_string_set_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_string_set_value(Sass_Value v, StringUtf8 value);

        [DllImport(LibSassDll, EntryPoint = "sass_string_is_quoted", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_string_is_quoted(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_string_set_quoted", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_string_set_quoted(Sass_Value v, bool quoted);

        /// <summary>
        /// Getters and setters for Sass_Boolean
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_boolean_get_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_boolean_get_value(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_boolean_set_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_boolean_set_value(Sass_Value v, bool value);

        /// <summary>
        /// Getters and setters for Sass_Color
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_color_get_r", CallingConvention = CallingConvention.Cdecl)]
        public static extern double sass_color_get_r(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_color_set_r", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_color_set_r(Sass_Value v, double r);

        [DllImport(LibSassDll, EntryPoint = "sass_color_get_g", CallingConvention = CallingConvention.Cdecl)]
        public static extern double sass_color_get_g(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_color_set_g", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_color_set_g(Sass_Value v, double g);

        [DllImport(LibSassDll, EntryPoint = "sass_color_get_b", CallingConvention = CallingConvention.Cdecl)]
        public static extern double sass_color_get_b(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_color_set_b", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_color_set_b(Sass_Value v, double b);

        [DllImport(LibSassDll, EntryPoint = "sass_color_get_a", CallingConvention = CallingConvention.Cdecl)]
        public static extern double sass_color_get_a(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_color_set_a", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_color_set_a(Sass_Value v, double a);

        /// <summary>
        /// Getter for the number of items in list
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_list_get_length", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_list_get_length(Sass_Value v);

        /// <summary>
        /// Getters and setters for Sass_List
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_list_get_separator", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Separator sass_list_get_separator(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_list_set_separator", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_list_set_separator(Sass_Value v, Sass_Separator value);

        [DllImport(LibSassDll, EntryPoint = "sass_list_get_is_bracketed", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_list_get_is_bracketed(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_list_set_is_bracketed", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_list_set_is_bracketed(Sass_Value v, bool value);

        /// <summary>
        /// Getters and setters for Sass_List values
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_list_get_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_list_get_value(Sass_Value v, size_t i);

        [DllImport(LibSassDll, EntryPoint = "sass_list_set_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_list_set_value(Sass_Value v, size_t i, Sass_Value value);

        /// <summary>
        /// Getter for the number of items in map
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_map_get_length", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_map_get_length(Sass_Value v);

        /// <summary>
        /// Getters and setters for Sass_Map keys and values
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_map_get_key", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_map_get_key(Sass_Value v, size_t i);

        [DllImport(LibSassDll, EntryPoint = "sass_map_set_key", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_map_set_key(Sass_Value v, size_t i, Sass_Value param2);

        [DllImport(LibSassDll, EntryPoint = "sass_map_get_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_map_get_value(Sass_Value v, size_t i);

        [DllImport(LibSassDll, EntryPoint = "sass_map_set_value", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_map_set_value(Sass_Value v, size_t i, Sass_Value param2);

        /// <summary>
        /// Getters and setters for Sass_Error
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_error_get_message", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_error_get_message(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_error_set_message", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_error_set_message(Sass_Value v, StringUtf8 msg);

        /// <summary>
        /// Getters and setters for Sass_Warning
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_warning_get_message", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_warning_get_message(Sass_Value v);

        [DllImport(LibSassDll, EntryPoint = "sass_warning_set_message", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_warning_set_message(Sass_Value v, StringUtf8 msg);

        public partial struct Sass_Env
        {
            public Sass_Env(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Callee
        {
            public Sass_Callee(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Import
        {
            public Sass_Import(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Options
        {
            public Sass_Options(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Compiler
        {
            public Sass_Compiler(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Importer
        {
            public Sass_Importer(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Function
        {
            public Sass_Function(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Env_Frame
        {
            public Sass_Env_Frame(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Callee_Entry
        {
            public Sass_Callee_Entry(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Import_Entry
        {
            public Sass_Import_Entry(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Import_List
        {
            public Sass_Import_List(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Importer_Entry
        {
            public Sass_Importer_Entry(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Importer_List
        {
            public Sass_Importer_List(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Importer_Fn
        {
            public Sass_Importer_Fn(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Function_Entry
        {
            public Sass_Function_Entry(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Function_List
        {
            public Sass_Function_List(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Function_Fn
        {
            public Sass_Function_Fn(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public enum Sass_Callee_Type
        {
            SASS_CALLEE_MIXIN = 0,
            SASS_CALLEE_FUNCTION = 1,
            SASS_CALLEE_C_FUNCTION = 2,
        }

        /// <summary>
        /// Creator for sass custom importer return argument list
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_make_importer_list", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Importer_List sass_make_importer_list(size_t length);

        [DllImport(LibSassDll, EntryPoint = "sass_importer_get_list_entry", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Importer_Entry sass_importer_get_list_entry(Sass_Importer_List list, size_t idx);

        [DllImport(LibSassDll, EntryPoint = "sass_importer_set_list_entry", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_importer_set_list_entry(Sass_Importer_List list, size_t idx, Sass_Importer_Entry entry);

        [DllImport(LibSassDll, EntryPoint = "sass_delete_importer_list", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_delete_importer_list(Sass_Importer_List list);

        /// <summary>
        /// Creators for custom importer callback (with some additional pointer) The pointer
        /// is mostly used to store the callback into the actual binding
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_make_importer", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Importer_Entry sass_make_importer(Sass_Importer_Fn importer, double priority, IntPtr cookie);

        /// <summary>
        /// Getters for import function descriptors
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_importer_get_function", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Importer_Fn sass_importer_get_function(Sass_Importer_Entry cb);

        [DllImport(LibSassDll, EntryPoint = "sass_importer_get_priority", CallingConvention = CallingConvention.Cdecl)]
        public static extern double sass_importer_get_priority(Sass_Importer_Entry cb);

        [DllImport(LibSassDll, EntryPoint = "sass_importer_get_cookie", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sass_importer_get_cookie(Sass_Importer_Entry cb);

        /// <summary>
        /// Deallocator for associated memory
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_delete_importer", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_delete_importer(Sass_Importer_Entry cb);

        /// <summary>
        /// Creator for sass custom importer return argument list
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_make_import_list", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Import_List sass_make_import_list(size_t length);

        /// <summary>
        /// Creator for a single import entry returned by the custom importer inside the
        /// list
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_make_import_entry", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Import_Entry sass_make_import_entry(StringUtf8 path, StringUtf8 source, StringUtf8 srcmap);

        [DllImport(LibSassDll, EntryPoint = "sass_make_import", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Import_Entry sass_make_import(StringUtf8 imp_path, StringUtf8 abs_base, StringUtf8 source, StringUtf8 srcmap);

        /// <summary>
        /// set error message to abort import and to print out a message (path from existing
        /// object is used in output)
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_import_set_error", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Import_Entry sass_import_set_error(Sass_Import_Entry import, StringUtf8 message, size_t line, size_t col);

        /// <summary>
        /// Setters to insert an entry into the import list (you may also use [] access
        /// directly) Since we are dealing with pointers they should have a guaranteed and
        /// fixed size
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_import_set_list_entry", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_import_set_list_entry(Sass_Import_List list, size_t idx, Sass_Import_Entry entry);

        [DllImport(LibSassDll, EntryPoint = "sass_import_get_list_entry", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Import_Entry sass_import_get_list_entry(Sass_Import_List list, size_t idx);

        /// <summary>
        /// Getters for callee entry
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_callee_get_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_callee_get_name(Sass_Callee_Entry param0);

        [DllImport(LibSassDll, EntryPoint = "sass_callee_get_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_callee_get_path(Sass_Callee_Entry param0);

        [DllImport(LibSassDll, EntryPoint = "sass_callee_get_line", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_callee_get_line(Sass_Callee_Entry param0);

        [DllImport(LibSassDll, EntryPoint = "sass_callee_get_column", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_callee_get_column(Sass_Callee_Entry param0);

        [DllImport(LibSassDll, EntryPoint = "sass_callee_get_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Callee_Type sass_callee_get_type(Sass_Callee_Entry param0);

        [DllImport(LibSassDll, EntryPoint = "sass_callee_get_env", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Env_Frame sass_callee_get_env(Sass_Callee_Entry param0);

        /// <summary>
        /// Getters and Setters for environments (lexical, local and global)
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_env_get_lexical", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_env_get_lexical(Sass_Env_Frame param0, StringUtf8 param1);

        [DllImport(LibSassDll, EntryPoint = "sass_env_set_lexical", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_env_set_lexical(Sass_Env_Frame param0, StringUtf8 param1, Sass_Value param2);

        [DllImport(LibSassDll, EntryPoint = "sass_env_get_local", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_env_get_local(Sass_Env_Frame param0, StringUtf8 param1);

        [DllImport(LibSassDll, EntryPoint = "sass_env_set_local", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_env_set_local(Sass_Env_Frame param0, StringUtf8 param1, Sass_Value param2);

        [DllImport(LibSassDll, EntryPoint = "sass_env_get_global", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Value sass_env_get_global(Sass_Env_Frame param0, StringUtf8 param1);

        [DllImport(LibSassDll, EntryPoint = "sass_env_set_global", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_env_set_global(Sass_Env_Frame param0, StringUtf8 param1, Sass_Value param2);

        /// <summary>
        /// Getters for import entry
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_import_get_imp_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_import_get_imp_path(Sass_Import_Entry param0);

        [DllImport(LibSassDll, EntryPoint = "sass_import_get_abs_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_import_get_abs_path(Sass_Import_Entry param0);

        [DllImport(LibSassDll, EntryPoint = "sass_import_get_source", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_import_get_source(Sass_Import_Entry param0);

        [DllImport(LibSassDll, EntryPoint = "sass_import_get_srcmap", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_import_get_srcmap(Sass_Import_Entry param0);

        /// <summary>
        /// Explicit functions to take ownership of these items The property on our struct
        /// will be reset to NULL
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_import_take_source", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_import_take_source(Sass_Import_Entry param0);

        [DllImport(LibSassDll, EntryPoint = "sass_import_take_srcmap", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_import_take_srcmap(Sass_Import_Entry param0);

        /// <summary>
        /// Getters from import error entry
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_import_get_error_line", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_import_get_error_line(Sass_Import_Entry param0);

        [DllImport(LibSassDll, EntryPoint = "sass_import_get_error_column", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_import_get_error_column(Sass_Import_Entry param0);

        [DllImport(LibSassDll, EntryPoint = "sass_import_get_error_message", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_import_get_error_message(Sass_Import_Entry param0);

        /// <summary>
        /// Deallocator for associated memory (incl. entries)
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_delete_import_list", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_delete_import_list(Sass_Import_List param0);

        /// <summary>
        /// Just in case we have some stray import structs
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_delete_import", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_delete_import(Sass_Import_Entry param0);

        /// <summary>
        /// Creators for sass function list and function descriptors
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_make_function_list", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Function_List sass_make_function_list(size_t length);

        [DllImport(LibSassDll, EntryPoint = "sass_make_function", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Function_Entry sass_make_function(StringUtf8 signature, Sass_Function_Fn cb, IntPtr cookie);

        [DllImport(LibSassDll, EntryPoint = "sass_delete_function", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_delete_function(Sass_Function_Entry entry);

        [DllImport(LibSassDll, EntryPoint = "sass_delete_function_list", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_delete_function_list(Sass_Function_List list);

        /// <summary>
        /// Setters and getters for callbacks on function lists
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_function_get_list_entry", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Function_Entry sass_function_get_list_entry(Sass_Function_List list, size_t pos);

        [DllImport(LibSassDll, EntryPoint = "sass_function_set_list_entry", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_function_set_list_entry(Sass_Function_List list, size_t pos, Sass_Function_Entry cb);

        /// <summary>
        /// Getters for custom function descriptors
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_function_get_signature", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_function_get_signature(Sass_Function_Entry cb);

        [DllImport(LibSassDll, EntryPoint = "sass_function_get_function", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Function_Fn sass_function_get_function(Sass_Function_Entry cb);

        [DllImport(LibSassDll, EntryPoint = "sass_function_get_cookie", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sass_function_get_cookie(Sass_Function_Entry cb);

        public partial struct Sass_Context
        {
            public Sass_Context(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_File_Context
        {
            public Sass_File_Context(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public partial struct Sass_Data_Context
        {
            public Sass_Data_Context(IntPtr pointer)
            {
                Pointer = pointer;
            }
            public IntPtr Pointer { get; }
        }

        public enum Sass_Compiler_State
        {
            SASS_COMPILER_CREATED = 0,
            SASS_COMPILER_PARSED = 1,
            SASS_COMPILER_EXECUTED = 2,
        }

        /// <summary>
        /// Create and initialize an option struct
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_make_options", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Options sass_make_options();

        /// <summary>
        /// Create and initialize a specific context
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_make_file_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_File_Context sass_make_file_context(StringUtf8 input_path);

        [DllImport(LibSassDll, EntryPoint = "sass_make_data_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Data_Context sass_make_data_context(StringUtf8 source_string);

        /// <summary>
        /// Call the compilation step for the specific context
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_compile_file_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sass_compile_file_context(Sass_File_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_compile_data_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sass_compile_data_context(Sass_Data_Context ctx);

        /// <summary>
        /// Create a sass compiler instance for more control
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_make_file_compiler", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Compiler sass_make_file_compiler(Sass_File_Context file_ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_make_data_compiler", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Compiler sass_make_data_compiler(Sass_Data_Context data_ctx);

        /// <summary>
        /// Execute the different compilation steps individually Usefull if you only want to
        /// query the included files
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_compiler_parse", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sass_compiler_parse(Sass_Compiler compiler);

        [DllImport(LibSassDll, EntryPoint = "sass_compiler_execute", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sass_compiler_execute(Sass_Compiler compiler);

        /// <summary>
        /// Release all memory allocated with the compiler This does _not_ include any
        /// contexts or options
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_delete_compiler", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_delete_compiler(Sass_Compiler compiler);

        [DllImport(LibSassDll, EntryPoint = "sass_delete_options", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_delete_options(Sass_Options options);

        /// <summary>
        /// Release all memory allocated and also ourself
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_delete_file_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_delete_file_context(Sass_File_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_delete_data_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_delete_data_context(Sass_Data_Context ctx);

        /// <summary>
        /// Getters for context from specific implementation
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_file_context_get_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Context sass_file_context_get_context(Sass_File_Context file_ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_data_context_get_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Context sass_data_context_get_context(Sass_Data_Context data_ctx);

        /// <summary>
        /// Getters for Context_Options from Sass_Context
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_context_get_options", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Options sass_context_get_options(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_file_context_get_options", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Options sass_file_context_get_options(Sass_File_Context file_ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_data_context_get_options", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Options sass_data_context_get_options(Sass_Data_Context data_ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_file_context_set_options", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_file_context_set_options(Sass_File_Context file_ctx, Sass_Options opt);

        [DllImport(LibSassDll, EntryPoint = "sass_data_context_set_options", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_data_context_set_options(Sass_Data_Context data_ctx, Sass_Options opt);

        /// <summary>
        /// Getters for Context_Option values
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_option_get_precision", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sass_option_get_precision(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_output_style", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Output_Style sass_option_get_output_style(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_source_comments", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_option_get_source_comments(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_source_map_embed", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_option_get_source_map_embed(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_source_map_contents", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_option_get_source_map_contents(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_source_map_file_urls", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_option_get_source_map_file_urls(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_omit_source_map_url", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_option_get_omit_source_map_url(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_is_indented_syntax_src", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool sass_option_get_is_indented_syntax_src(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_indent", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_option_get_indent(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_linefeed", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_option_get_linefeed(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_input_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_option_get_input_path(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_output_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_option_get_output_path(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_source_map_file", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_option_get_source_map_file(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_source_map_root", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_option_get_source_map_root(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_c_headers", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Importer_List sass_option_get_c_headers(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_c_importers", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Importer_List sass_option_get_c_importers(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_c_functions", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Function_List sass_option_get_c_functions(Sass_Options options);

        /// <summary>
        /// Setters for Context_Option values
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_option_set_precision", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_precision(Sass_Options options, int precision);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_output_style", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_output_style(Sass_Options options, Sass_Output_Style output_style);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_source_comments", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_source_comments(Sass_Options options, bool source_comments);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_source_map_embed", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_source_map_embed(Sass_Options options, bool source_map_embed);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_source_map_contents", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_source_map_contents(Sass_Options options, bool source_map_contents);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_source_map_file_urls", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_source_map_file_urls(Sass_Options options, bool source_map_file_urls);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_omit_source_map_url", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_omit_source_map_url(Sass_Options options, bool omit_source_map_url);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_is_indented_syntax_src", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_is_indented_syntax_src(Sass_Options options, bool is_indented_syntax_src);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_indent", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_indent(Sass_Options options, StringUtf8 indent);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_linefeed", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_linefeed(Sass_Options options, StringUtf8 linefeed);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_input_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_input_path(Sass_Options options, StringUtf8 input_path);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_output_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_output_path(Sass_Options options, StringUtf8 output_path);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_plugin_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_plugin_path(Sass_Options options, StringUtf8 plugin_path);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_include_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_include_path(Sass_Options options, StringUtf8 include_path);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_source_map_file", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_source_map_file(Sass_Options options, StringUtf8 source_map_file);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_source_map_root", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_source_map_root(Sass_Options options, StringUtf8 source_map_root);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_c_headers", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_c_headers(Sass_Options options, Sass_Importer_List c_headers);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_c_importers", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_c_importers(Sass_Options options, Sass_Importer_List c_importers);

        [DllImport(LibSassDll, EntryPoint = "sass_option_set_c_functions", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_set_c_functions(Sass_Options options, Sass_Function_List c_functions);

        /// <summary>
        /// Getters for Sass_Context values
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_context_get_output_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_get_output_string(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_get_error_status", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sass_context_get_error_status(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_get_error_json", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_get_error_json(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_get_error_text", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_get_error_text(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_get_error_message", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_get_error_message(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_get_error_file", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_get_error_file(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_get_error_src", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_get_error_src(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_get_error_line", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_context_get_error_line(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_get_error_column", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_context_get_error_column(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_get_source_map_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_get_source_map_string(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_get_included_files", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sass_context_get_included_files(Sass_Context ctx);

        /// <summary>
        /// Getters for options include path array
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_option_get_include_path_size", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_option_get_include_path_size(Sass_Options options);

        [DllImport(LibSassDll, EntryPoint = "sass_option_get_include_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_option_get_include_path(Sass_Options options, size_t i);

        /// <summary>
        /// Calculate the size of the stored null terminated array
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_context_get_included_files_size", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_context_get_included_files_size(Sass_Context ctx);

        /// <summary>
        /// Take ownership of memory (value on context is set to 0)
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_context_take_error_json", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_take_error_json(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_take_error_text", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_take_error_text(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_take_error_message", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_take_error_message(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_take_error_file", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_take_error_file(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_take_output_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_take_output_string(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_take_source_map_string", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_context_take_source_map_string(Sass_Context ctx);

        [DllImport(LibSassDll, EntryPoint = "sass_context_take_included_files", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sass_context_take_included_files(Sass_Context ctx);

        /// <summary>
        /// Getters for Sass_Compiler options
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_compiler_get_state", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Compiler_State sass_compiler_get_state(Sass_Compiler compiler);

        [DllImport(LibSassDll, EntryPoint = "sass_compiler_get_context", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Context sass_compiler_get_context(Sass_Compiler compiler);

        [DllImport(LibSassDll, EntryPoint = "sass_compiler_get_options", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Options sass_compiler_get_options(Sass_Compiler compiler);

        [DllImport(LibSassDll, EntryPoint = "sass_compiler_get_import_stack_size", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_compiler_get_import_stack_size(Sass_Compiler compiler);

        [DllImport(LibSassDll, EntryPoint = "sass_compiler_get_last_import", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Import_Entry sass_compiler_get_last_import(Sass_Compiler compiler);

        [DllImport(LibSassDll, EntryPoint = "sass_compiler_get_import_entry", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Import_Entry sass_compiler_get_import_entry(Sass_Compiler compiler, size_t idx);

        [DllImport(LibSassDll, EntryPoint = "sass_compiler_get_callee_stack_size", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t sass_compiler_get_callee_stack_size(Sass_Compiler compiler);

        [DllImport(LibSassDll, EntryPoint = "sass_compiler_get_last_callee", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Callee_Entry sass_compiler_get_last_callee(Sass_Compiler compiler);

        [DllImport(LibSassDll, EntryPoint = "sass_compiler_get_callee_entry", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sass_Callee_Entry sass_compiler_get_callee_entry(Sass_Compiler compiler, size_t idx);

        /// <summary>
        /// Push function for import extenions
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_option_push_import_extension", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_push_import_extension(Sass_Options options, StringUtf8 ext);

        /// <summary>
        /// Push function for paths (no manipulation support for now)
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_option_push_plugin_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_push_plugin_path(Sass_Options options, StringUtf8 path);

        [DllImport(LibSassDll, EntryPoint = "sass_option_push_include_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sass_option_push_include_path(Sass_Options options, StringUtf8 path);

        /// <summary>
        /// Resolve a file via the given include paths in the sass option struct find_file
        /// looks for the exact file name while find_include does a regular sass include
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_find_file", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_find_file(StringUtf8 path, Sass_Options opt);

        [DllImport(LibSassDll, EntryPoint = "sass_find_include", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_find_include(StringUtf8 path, Sass_Options opt);

        /// <summary>
        /// Resolve a file relative to last import or include paths in the sass option
        /// struct find_file looks for the exact file name while find_include does a
        /// regular sass include
        /// </summary>
        [DllImport(LibSassDll, EntryPoint = "sass_compiler_find_file", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_compiler_find_file(StringUtf8 path, Sass_Compiler compiler);

        [DllImport(LibSassDll, EntryPoint = "sass_compiler_find_include", CallingConvention = CallingConvention.Cdecl)]
        public static extern StringUtf8 sass_compiler_find_include(StringUtf8 path, Sass_Compiler compiler);
    }
}
