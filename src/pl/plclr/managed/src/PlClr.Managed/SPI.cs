using System;
using System.Collections.Generic;
using System.Text;

namespace PlClr
{
    public static class SPI
    {
        public static int SPI_connect()
        {
            throw new NotImplementedException();
        }

        public static int SPI_connect(int options)
        {
            throw new NotImplementedException();
        }

        public static int SPI_finish()
        {
            throw new NotImplementedException();
        }

        public static int SPI_execute(string command, bool readOnly, int count)
        {
            throw new NotImplementedException();
        }

        public static int SPI_exec(string command, long count)
        {
            throw new NotImplementedException();
        }

        public static int SPI_execute_with_args(string command, int nArgs, uint[] argTypes, object[] values, byte[] nulls, bool readOnly, int count)
        {
            throw new NotImplementedException();
        }

        public static IntPtr SPI_prepare(string command, int nArgs, uint[] argTypes)
        {
            throw new NotImplementedException();
        }

        public static IntPtr SPI_prepare_cursor(string command, int nargs, uint[] argtypes, int cursorOptions)
        {
            throw new NotImplementedException();
        }

        // ToDo: think about signature
        //public static IntPtr SPI_prepare_params(string command, ParserSetupHook parserSetup, IntPtr parserSetupArg, int cursorOptions)
        //{
        //    throw new NotImplementedException();
        //}

        public static int SPI_getargcount(IntPtr plan)
        {
            throw new NotImplementedException();
        }

        public static uint SPI_getargtypeid(IntPtr plan, int argIndex)
        {
            throw new NotImplementedException();
        }

        public static bool SPI_is_cursor_plan(IntPtr plan)
        {
            throw new NotImplementedException();
        }

        public static int SPI_execute_plan(IntPtr plan, object[] values, byte[] nulls, bool read_only, long count)
        {
            throw new NotImplementedException();
        }

        // ToDo: think about signature
        //public static int SPI_execute_plan_with_paramlist(IntPtr plan, ParamListInfo params, bool read_only, long count)
        //{
        //    throw new NotImplementedException();
        //}

        public static int SPI_execp(IntPtr plan, object[] values, byte[] nulls, long count)
        {
            throw new NotImplementedException();
        }

        public static IntPtr SPI_cursor_open(string name, IntPtr plan, object[] values, byte[] nulls, bool read_only)
        {
            throw new NotImplementedException();
        }

        public static IntPtr SPI_cursor_open_with_args(string name, string command, int nargs, uint[] argtypes, object[] values, byte[] nulls, bool read_only, int cursorOptions)
        {
            throw new NotImplementedException();
        }

        // ToDo: think about signature
        //public static IntPtr SPI_cursor_open_with_paramlist(string name, IntPtr plan, ParamListInfo params, bool read_only)
        //{
        //    throw new NotImplementedException();
        //}

        public static IntPtr SPI_cursor_find(string name)
        {
            throw new NotImplementedException();
        }

        public static void SPI_cursor_fetch(IntPtr portal, bool forward, long count)
        {
            throw new NotImplementedException();
        }

        public static void SPI_cursor_move(IntPtr portal, bool forward, long count)
        {
            throw new NotImplementedException();
        }

        // ToDo: think about signature
        //public static void SPI_scroll_cursor_fetch(IntPtr portal, FetchDirection direction, long count)
        //{
        //    throw new NotImplementedException();
        //}

        // ToDo: think about signature
        //public static void SPI_scroll_cursor_move(IntPtr portal, FetchDirection direction, long count)
        //{
        //    throw new NotImplementedException();
        //}

        public static void SPI_cursor_close(IntPtr portal)
        {
            throw new NotImplementedException();
        }

        public static int SPI_keepplan(IntPtr plan)
        {
            throw new NotImplementedException();
        }

        public static IntPtr SPI_saveplan(IntPtr plan)
        {
            throw new NotImplementedException();
        }

        // ToDo: think about signature
        //public static int SPI_register_relation(EphemeralNamedRelation enr)
        //{
        //    throw new NotImplementedException();
        //}

        public static int SPI_unregister_relation(string name)
        {
            throw new NotImplementedException();
        }

        // ToDo: think about signature
        //public static int SPI_register_trigger_data(TriggerData *tdata)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
