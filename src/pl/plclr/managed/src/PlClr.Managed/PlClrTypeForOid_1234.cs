using System;
using static PlClr.Globals;

namespace PlClr
{
    public class PlClrTypeForOid_1234
    {
        public PlClrTypeForOid_1234(string name, int salary)
        {
            this.name = name;
            this.salary = salary;
        }

        public string name { get; set; }
        public int salary { get; set; }

        public static PlClrTypeForOid_1234 Get1234(IntPtr datum)
        {
            var heapTupleHeader = BackendFunctions.DeToastDatum(datum);

            var ret = new PlClrTypeForOid_1234(
                BackendFunctions.GetText(BackendFunctions.GetAttributeByNum(heapTupleHeader, 1, out _)),
                BackendFunctions.GetInt32(BackendFunctions.GetAttributeByNum(heapTupleHeader, 2, out _))
            );

            if (heapTupleHeader != datum)
                ServerMemory.PFree(heapTupleHeader);

            return ret;
        }
    }

    public class PlClrTypeForOid_2345
    {
        public PlClrTypeForOid_2345(string? name, int? salary)
        {
            this.name = name;
            this.salary = salary;
        }

        public string? name { get; set; }
        public int? salary { get; set; }

        public static PlClrTypeForOid_2345 Get2345(IntPtr datum)
        {
            var heapTupleHeader = BackendFunctions.DeToastDatum(datum);

            var namePtr = BackendFunctions.GetAttributeByNum(heapTupleHeader, 1, out var nameIsNull);
            var salaryPtr = BackendFunctions.GetAttributeByNum(heapTupleHeader, 2, out var salaryIsNull);

            var ret = new PlClrTypeForOid_2345(
                nameIsNull ? null : BackendFunctions.GetText(namePtr),
                salaryIsNull ? (int?)null : BackendFunctions.GetInt32(salaryPtr)
            );

            if (heapTupleHeader != datum)
                ServerMemory.PFree(heapTupleHeader);

            return ret;
        }
    }
}