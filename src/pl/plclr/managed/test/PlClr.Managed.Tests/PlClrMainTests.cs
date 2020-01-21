using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Xunit;

namespace PlClr.Managed.Tests
{
    public class PlClrMainTests
    {
        [Fact]
        public void SetupSuccess()
        {
            using var h = new TestHelper();
            var (unmanagedInterfacePointer, unmanagedInterfaceSize) = h.GetUnmanagedInterface();

            var resultPtr = PlClrMain.Setup(unmanagedInterfacePointer, unmanagedInterfaceSize);
            
            Assert.NotEqual(IntPtr.Zero, resultPtr);
            var managedInterface = (PlClrManagedInterface)System.Runtime.InteropServices.Marshal.PtrToStructure(resultPtr, typeof(PlClrManagedInterface))!;
            Assert.NotEqual(IntPtr.Zero, managedInterface.CompileFunctionPtr);
            Assert.NotEqual(IntPtr.Zero, managedInterface.ExecuteFunctionPtr);
            var compileFunctionDelegate =
                System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PlClrMainDelegate>(managedInterface
                    .CompileFunctionPtr);
            Assert.Equal(PlClrMain.Compile, compileFunctionDelegate);
            var executeFunctionDelegate =
                System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PlClrMainDelegate>(managedInterface
                    .ExecuteFunctionPtr);
            Assert.Equal(PlClrMain.Execute, executeFunctionDelegate);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Empty(h.Log.ConsoleError);
            Assert.Empty(h.Log.ELogMessages);
        }

        [Fact]
        public void SetupNullPtr()
        {
            using var h = new TestHelper();
            var (_, unmanagedInterfaceSize) = h.GetUnmanagedInterface();

            var resultPtr = PlClrMain.Setup(IntPtr.Zero, unmanagedInterfaceSize);

            Assert.Equal(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Equal($"Argument arg must not be NULL{Environment.NewLine}", h.Log.ConsoleError);
            Assert.Empty(h.Log.ELogMessages);
        }

        [Fact]
        public void SetupZeroLength()
        {
            using var h = new TestHelper();
            var (unmanagedInterfacePointer, unmanagedInterfaceSize) = h.GetUnmanagedInterface();

            var resultPtr = PlClrMain.Setup(unmanagedInterfacePointer, 0);

            Assert.Equal(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Equal($"Argument argLength is 0 but is expected to be greater than or equal to {unmanagedInterfaceSize}{Environment.NewLine}", h.Log.ConsoleError);
            Assert.Empty(h.Log.ELogMessages);
        }

        [Fact]
        public void SetupNoPAllocPtr()
        {
            using var h = new TestHelper();
            PlClrUnmanagedInterface i;
            i.ELogFunctionPtr = h.Log.GetELogFunctionPointer();
            i.EReportFunctionPtr = h.Log.GetEReportFunctionPointer();
            i.PFreeFunctionPtr = h.MemoryContext.GetPFreeFunctionPointer();
            i.PAlloc0FunctionPtr = h.MemoryContext.GetPAlloc0FunctionPointer();
            i.PAllocFunctionPtr = IntPtr.Zero;
            i.RePAllocFunctionPtr = h.MemoryContext.GetRePAllocFunctionPointer();
            i.GetTextFunctionPtr = h.Function.GetGetTextFunctionPointer();
            var unmanagedInterfaceSize = System.Runtime.InteropServices.Marshal.SizeOf<PlClrUnmanagedInterface>();
            var unmanagedInterfacePointer = h.AllocCoTaskMem(unmanagedInterfaceSize);
            System.Runtime.InteropServices.Marshal.StructureToPtr(i, unmanagedInterfacePointer, false);

            var resultPtr = PlClrMain.Setup(unmanagedInterfacePointer, unmanagedInterfaceSize);

            Assert.Equal(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Equal($"Field PAllocFunctionPtr in struct PlClrUnmanagedInterface must not be NULL{Environment.NewLine}", h.Log.ConsoleError);
            Assert.Empty(h.Log.ELogMessages);
        }

        [Fact]
        public void SetupNoPAlloc0Ptr()
        {
            using var h = new TestHelper();
            PlClrUnmanagedInterface i;
            i.ELogFunctionPtr = h.Log.GetELogFunctionPointer();
            i.EReportFunctionPtr = h.Log.GetEReportFunctionPointer();
            i.PFreeFunctionPtr = h.MemoryContext.GetPFreeFunctionPointer();
            i.PAlloc0FunctionPtr = IntPtr.Zero;
            i.PAllocFunctionPtr = h.MemoryContext.GetPAllocFunctionPointer();
            i.RePAllocFunctionPtr = h.MemoryContext.GetRePAllocFunctionPointer();
            i.GetTextFunctionPtr = h.Function.GetGetTextFunctionPointer();
            var unmanagedInterfaceSize = System.Runtime.InteropServices.Marshal.SizeOf<PlClrUnmanagedInterface>();
            var unmanagedInterfacePointer = h.AllocCoTaskMem(unmanagedInterfaceSize);
            System.Runtime.InteropServices.Marshal.StructureToPtr(i, unmanagedInterfacePointer, false);

            var resultPtr = PlClrMain.Setup(unmanagedInterfacePointer, unmanagedInterfaceSize);

            Assert.Equal(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Equal($"Field PAlloc0FunctionPtr in struct PlClrUnmanagedInterface must not be NULL{Environment.NewLine}", h.Log.ConsoleError);
            Assert.Empty(h.Log.ELogMessages);
        }

        [Fact]
        public void SetupNoRePAllocPtr()
        {
            using var h = new TestHelper();
            PlClrUnmanagedInterface i;
            i.ELogFunctionPtr = h.Log.GetELogFunctionPointer();
            i.EReportFunctionPtr = h.Log.GetEReportFunctionPointer();
            i.PFreeFunctionPtr = h.MemoryContext.GetPFreeFunctionPointer();
            i.PAlloc0FunctionPtr = h.MemoryContext.GetPAllocFunctionPointer();
            i.PAllocFunctionPtr = h.MemoryContext.GetPAllocFunctionPointer();
            i.RePAllocFunctionPtr = IntPtr.Zero;
            i.GetTextFunctionPtr = h.Function.GetGetTextFunctionPointer();
            var unmanagedInterfaceSize = System.Runtime.InteropServices.Marshal.SizeOf<PlClrUnmanagedInterface>();
            var unmanagedInterfacePointer = h.AllocCoTaskMem(unmanagedInterfaceSize);
            System.Runtime.InteropServices.Marshal.StructureToPtr(i, unmanagedInterfacePointer, false);

            var resultPtr = PlClrMain.Setup(unmanagedInterfacePointer, unmanagedInterfaceSize);

            Assert.Equal(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Equal($"Field RePAllocFunctionPtr in struct PlClrUnmanagedInterface must not be NULL{Environment.NewLine}", h.Log.ConsoleError);
            Assert.Empty(h.Log.ELogMessages);
        }

        [Fact]
        public void SetupNoPFreePtr()
        {
            using var h = new TestHelper();
            PlClrUnmanagedInterface i;
            i.ELogFunctionPtr = h.Log.GetELogFunctionPointer();
            i.EReportFunctionPtr = h.Log.GetEReportFunctionPointer();
            i.PFreeFunctionPtr = IntPtr.Zero;
            i.PAlloc0FunctionPtr = h.MemoryContext.GetPAllocFunctionPointer();
            i.PAllocFunctionPtr = h.MemoryContext.GetPAllocFunctionPointer();
            i.RePAllocFunctionPtr = h.MemoryContext.GetRePAllocFunctionPointer();
            i.GetTextFunctionPtr = h.Function.GetGetTextFunctionPointer();
            var unmanagedInterfaceSize = System.Runtime.InteropServices.Marshal.SizeOf<PlClrUnmanagedInterface>();
            var unmanagedInterfacePointer = h.AllocCoTaskMem(unmanagedInterfaceSize);
            System.Runtime.InteropServices.Marshal.StructureToPtr(i, unmanagedInterfacePointer, false);

            var resultPtr = PlClrMain.Setup(unmanagedInterfacePointer, unmanagedInterfaceSize);

            Assert.Equal(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Equal($"Field PFreeFunctionPtr in struct PlClrUnmanagedInterface must not be NULL{Environment.NewLine}", h.Log.ConsoleError);
            Assert.Empty(h.Log.ELogMessages);
        }

        [Fact]
        public void SetupNoELogPtr()
        {
            using var h = new TestHelper();
            PlClrUnmanagedInterface i;
            i.ELogFunctionPtr = IntPtr.Zero;
            i.EReportFunctionPtr = h.Log.GetEReportFunctionPointer();
            i.PFreeFunctionPtr = h.MemoryContext.GetPFreeFunctionPointer();
            i.PAlloc0FunctionPtr = h.MemoryContext.GetPAllocFunctionPointer();
            i.PAllocFunctionPtr = h.MemoryContext.GetPAllocFunctionPointer();
            i.RePAllocFunctionPtr = h.MemoryContext.GetRePAllocFunctionPointer();
            i.GetTextFunctionPtr = h.Function.GetGetTextFunctionPointer();
            var unmanagedInterfaceSize = System.Runtime.InteropServices.Marshal.SizeOf<PlClrUnmanagedInterface>();
            var unmanagedInterfacePointer = h.AllocCoTaskMem(unmanagedInterfaceSize);
            System.Runtime.InteropServices.Marshal.StructureToPtr(i, unmanagedInterfacePointer, false);

            var resultPtr = PlClrMain.Setup(unmanagedInterfacePointer, unmanagedInterfaceSize);

            Assert.Equal(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Equal($"Field ELogFunctionPtr in struct PlClrUnmanagedInterface must not be NULL{Environment.NewLine}", h.Log.ConsoleError);
            Assert.Empty(h.Log.ELogMessages);
        }

        [Fact]
        public void SetupUnexpectedException()
        {
            using var h = new TestHelper();
            PlClrUnmanagedInterface i;
            i.ELogFunctionPtr = h.Log.GetELogFunctionPointer();
            i.EReportFunctionPtr = h.Log.GetEReportFunctionPointer();
            i.PFreeFunctionPtr = h.MemoryContext.GetPFreeFunctionPointer();
            i.PAlloc0FunctionPtr = h.MemoryContext.GetPAlloc0FunctionPointer();
            i.PAllocFunctionPtr = h.MemoryContext.GetPAllocFunctionPointer(ThrowingPAlloc);
            i.RePAllocFunctionPtr = h.MemoryContext.GetRePAllocFunctionPointer();
            i.GetTextFunctionPtr = h.Function.GetGetTextFunctionPointer();
            var unmanagedInterfaceSize = System.Runtime.InteropServices.Marshal.SizeOf<PlClrUnmanagedInterface>();
            var unmanagedInterfacePointer = h.AllocCoTaskMem(unmanagedInterfaceSize);
            System.Runtime.InteropServices.Marshal.StructureToPtr(i, unmanagedInterfacePointer, false);

            var resultPtr = PlClrMain.Setup(unmanagedInterfacePointer, unmanagedInterfaceSize);

            Assert.Equal(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.StartsWith(
                $"An unexpected exception occured during PL/CLR setup: System.InvalidOperationException: This is a Test!",
                h.Log.ConsoleError);
            Assert.Empty(h.Log.ELogMessages);

            static IntPtr ThrowingPAlloc(ulong size) => throw new InvalidOperationException("This is a Test!");
        }

        [Fact]
        public void CompileSimpleSuccess()
        {
            using var h = new TestHelper();
            var (functionCompileInfoPointer, functionCompileInfoSize) = h.GetFunctionCompileInfo();

            var resultPtr = PlClrMain.Compile(functionCompileInfoPointer, functionCompileInfoSize);

            Assert.NotEqual(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Empty(h.Log.ConsoleError);
            Assert.Collection(h.Log.ELogMessages,
                entry =>
                {
                    Assert.Equal(SeverityLevel.Debug1, entry.Item1);
                    Assert.Equal(@"PL/CLR generated code:
using PlClr;
using System;

public static class PlClrClass_" + h.FunctionOidDefault + @"
{
	public static IntPtr Execute_TestFunction(NullableDatum[] values)
	{
		TestFunction();
		NullableDatum returnValue;
		returnValue.IsNull = false;
		returnValue.Value = IntPtr.Zero;

		return Marshal.StructureToPtrPalloc(returnValue);
	}

	private static void TestFunction()
	{
	}
}
", entry.Item2);
                });
        }

        [Fact]
        public void CompileNullPtr()
        {
            using var h = new TestHelper();
            var (_, functionCompileInfoSize) = h.GetFunctionCompileInfo();

            var resultPtr = PlClrMain.Compile(IntPtr.Zero, functionCompileInfoSize);

            Assert.Equal(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Empty(h.Log.ConsoleError);
            Assert.Collection(h.Log.ELogMessages,
                message =>
                {
                    Assert.Equal(SeverityLevel.Error, message.Item1);
                    Assert.Equal("Argument arg must not be NULL", message.Item2);
                }
                );
        }

        [Fact]
        public void CompileInt32InArgsWithoutNameSuccess()
        {
            using var h = new TestHelper();
            var (functionCompileInfoPointer, functionCompileInfoSize) = h.GetFunctionCompileInfo(
                name: "AddTwoIntegers",
                arguments: new []
                {
                    new Argument(TypeOid.Int32),
                    new Argument(TypeOid.Int32)
                },
                body:"if (arg1 == null || arg2 == null)\n\treturn null;\n\nreturn arg1 + arg2;",
                returnValueType: TypeOid.Int32
            );

            var resultPtr = PlClrMain.Compile(functionCompileInfoPointer, functionCompileInfoSize);

            Assert.NotEqual(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Empty(h.Log.ConsoleError);
            Assert.Collection(h.Log.ELogMessages,
                entry =>
                {
                    Assert.Equal(SeverityLevel.Debug1, entry.Item1);
                    Assert.Equal(@"PL/CLR generated code:
using PlClr;
using System;

public static class PlClrClass_" + h.FunctionOidDefault + @"
{
	public static IntPtr Execute_AddTwoIntegers(NullableDatum[] values)
	{
		var arg1 = values[0].IsNull ? (System.Int32?)null : ServerFunctions.GetInt32(values[0].Value);
		var arg2 = values[1].IsNull ? (System.Int32?)null : ServerFunctions.GetInt32(values[1].Value);

		var result = AddTwoIntegers(arg1, arg2);
		NullableDatum returnValue;
		if (result == null)
		{
			returnValue.IsNull = true;
			returnValue.Value = IntPtr.Zero;
		}
		else
		{
			returnValue.IsNull = false;
			returnValue.Value = ServerFunctions.GetDatum((System.Int32)result);
		}

		return Marshal.StructureToPtrPalloc(returnValue);
	}

	private static System.Int32? AddTwoIntegers(System.Int32? arg1, System.Int32? arg2)
	{
if (arg1 == null || arg2 == null)
	return null;

return arg1 + arg2;
	}
}
", entry.Item2);
                });
        }

        [Fact]
        public void CompileInt32InArgsWithoutNameStrictSuccess()
        {
            using var h = new TestHelper();
            var (functionCompileInfoPointer, functionCompileInfoSize) = h.GetFunctionCompileInfo(
                name: "AddTwoIntegers",
                arguments: new []
                {
                    new Argument(TypeOid.Int32),
                    new Argument(TypeOid.Int32)
                },
                body:"return arg1 + arg2;",
                returnValueType: TypeOid.Int32,
                isStrict: true
            );

            var resultPtr = PlClrMain.Compile(functionCompileInfoPointer, functionCompileInfoSize);

            Assert.NotEqual(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Empty(h.Log.ConsoleError);
            Assert.Collection(h.Log.ELogMessages,
                entry =>
                {
                    Assert.Equal(SeverityLevel.Debug1, entry.Item1);
                    Assert.Equal(@"PL/CLR generated code:
using PlClr;
using System;

public static class PlClrClass_" + h.FunctionOidDefault + @"
{
	public static IntPtr Execute_AddTwoIntegers(NullableDatum[] values)
	{
		var arg1 = ServerFunctions.GetInt32(values[0].Value);
		var arg2 = ServerFunctions.GetInt32(values[1].Value);

		var result = AddTwoIntegers(arg1, arg2);
		NullableDatum returnValue;
		returnValue.IsNull = false;
		returnValue.Value = ServerFunctions.GetDatum(result);

		return Marshal.StructureToPtrPalloc(returnValue);
	}

	private static System.Int32 AddTwoIntegers(System.Int32 arg1, System.Int32 arg2)
	{
return arg1 + arg2;
	}
}
", entry.Item2);
                });
        }

        [Fact]
        public void CompileInt32InArgsWithNameSuccess()
        {
            using var h = new TestHelper();
            var (functionCompileInfoPointer, functionCompileInfoSize) = h.GetFunctionCompileInfo(
                name: "AddTwoIntegers",
                arguments: new []
                {
                    new Argument(TypeOid.Int32, "first"),
                    new Argument(TypeOid.Int32, "second")
                },
                body:"if (first == null || second == null)\n\treturn null;\n\nreturn first + second;",
                returnValueType: TypeOid.Int32
            );

            var resultPtr = PlClrMain.Compile(functionCompileInfoPointer, functionCompileInfoSize);

            Assert.NotEqual(IntPtr.Zero, resultPtr);
            Assert.Empty(h.Log.ConsoleOut);
            Assert.Empty(h.Log.ConsoleError);
            Assert.Collection(h.Log.ELogMessages,
                entry =>
                {
                    Assert.Equal(SeverityLevel.Debug1, entry.Item1);
                    Assert.Equal(@"PL/CLR generated code:
using PlClr;
using System;

public static class PlClrClass_" + h.FunctionOidDefault + @"
{
	public static IntPtr Execute_AddTwoIntegers(NullableDatum[] values)
	{
		var first = values[0].IsNull ? (System.Int32?)null : ServerFunctions.GetInt32(values[0].Value);
		var second = values[1].IsNull ? (System.Int32?)null : ServerFunctions.GetInt32(values[1].Value);

		var result = AddTwoIntegers(first, second);
		NullableDatum returnValue;
		if (result == null)
		{
			returnValue.IsNull = true;
			returnValue.Value = IntPtr.Zero;
		}
		else
		{
			returnValue.IsNull = false;
			returnValue.Value = ServerFunctions.GetDatum((System.Int32)result);
		}

		return Marshal.StructureToPtrPalloc(returnValue);
	}

	private static System.Int32? AddTwoIntegers(System.Int32? first, System.Int32? second)
	{
if (first == null || second == null)
	return null;

return first + second;
	}
}
", entry.Item2);
                });
        }

    }
}
