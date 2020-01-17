using System;
using System.Runtime.ExceptionServices;
using System.Security;
using Xunit;

namespace PlClr.Managed.Tests.SelfTests
{
    public class MemoryContextTests
    {
        [Fact]
        void TotalBytesPAllocReportsCorrectValue()
        {
            using var memoryContext = new MemoryContext();

            var ptr = memoryContext.PAlloc(8ul);

            Assert.Equal(8ul, memoryContext.TotalBytesPAlloc);

            memoryContext.PAlloc(8ul);

            Assert.Equal(16ul, memoryContext.TotalBytesPAlloc);

            memoryContext.PAlloc0(16ul);

            Assert.Equal(16ul, memoryContext.TotalBytesPAlloc);

            memoryContext.RePAlloc(ptr, 16ul);

            Assert.Equal(16ul, memoryContext.TotalBytesPAlloc);
        }

        [Fact]
        void TotalBytesPAlloc0ReportsCorrectValue()
        {
            using var memoryContext = new MemoryContext();

            var ptr = memoryContext.PAlloc0(8ul);

            Assert.Equal(8ul, memoryContext.TotalBytesPAlloc0);

            memoryContext.PAlloc0(8ul);

            Assert.Equal(16ul, memoryContext.TotalBytesPAlloc0);

            memoryContext.PAlloc(16ul);

            Assert.Equal(16ul, memoryContext.TotalBytesPAlloc0);

            memoryContext.RePAlloc(ptr, 16ul);

            Assert.Equal(16ul, memoryContext.TotalBytesPAlloc0);
        }

        [Fact]
        void TotalBytesRePAllocReportsCorrectValue()
        {
            using var memoryContext = new MemoryContext();

            var ptr = memoryContext.PAlloc(8ul); 
            ptr = memoryContext.RePAlloc(ptr, 8ul);

            Assert.Equal(8ul, memoryContext.TotalBytesRePAlloc);

            ptr = memoryContext.RePAlloc(ptr, 4ul);

            Assert.Equal(12ul, memoryContext.TotalBytesRePAlloc);

            ptr = memoryContext.RePAlloc(ptr, 8ul);

            Assert.Equal(20ul, memoryContext.TotalBytesRePAlloc);

            memoryContext.PAlloc(16ul);

            Assert.Equal(20ul, memoryContext.TotalBytesRePAlloc);

            memoryContext.PAlloc0(16ul);

            Assert.Equal(20ul, memoryContext.TotalBytesRePAlloc);
        }

        [Fact]
        void TotalBytesRePAllocFreeReportsCorrectValue()
        {
            using var memoryContext = new MemoryContext();

            var ptr = memoryContext.PAlloc(8ul); 
            ptr = memoryContext.RePAlloc(ptr, 8ul);

            Assert.Equal(8ul, memoryContext.TotalBytesRePAllocFree);

            ptr = memoryContext.RePAlloc(ptr, 4ul);

            Assert.Equal(16ul, memoryContext.TotalBytesRePAllocFree);

            ptr = memoryContext.RePAlloc(ptr, 8ul);

            Assert.Equal(20ul, memoryContext.TotalBytesRePAllocFree);

            memoryContext.PAlloc(16ul);

            Assert.Equal(20ul, memoryContext.TotalBytesRePAllocFree);

            memoryContext.PAlloc0(16ul);

            Assert.Equal(20ul, memoryContext.TotalBytesRePAllocFree);
        }

        [Fact]
        void TotalBytesPFreeReportsCorrectValue()
        {
            using var memoryContext = new MemoryContext();

            var ptr = memoryContext.PAlloc(8ul);
            memoryContext.PFree(ptr);

            Assert.Equal(8ul, memoryContext.TotalBytesPFree);

            ptr = memoryContext.PAlloc(8ul);
            memoryContext.PFree(ptr);

            Assert.Equal(16ul, memoryContext.TotalBytesPFree);

            ptr = memoryContext.PAlloc0(16ul);
            memoryContext.RePAlloc(ptr, 16ul);

            Assert.Equal(16ul, memoryContext.TotalBytesPFree);
        }

        [Fact]
        void PAllocReturnsValidPointer()
        {
            using var memoryContext = new MemoryContext();

            var ptr1 = memoryContext.PAlloc(8ul);

            Assert.NotEqual(IntPtr.Zero,ptr1);
        }

        [Fact]
        void PAllocReturnsCorrectAddress()
        {
            using var memoryContext = new MemoryContext();

            var ptr1 = memoryContext.PAlloc(8ul);
            var ptr2 = memoryContext.PAlloc(8ul);

            Assert.NotEqual(IntPtr.Zero,ptr1);
            Assert.NotEqual(IntPtr.Zero,ptr2);
            Assert.NotEqual(ptr1, ptr2);
        }

        [Fact]
        void PAllocThrowsOnDisposedMemoryContext()
        {
            var memoryContext = new MemoryContext();
            memoryContext.Dispose();

            var exception = Assert.Throws<ObjectDisposedException>(() => memoryContext.PAlloc(8UL));

            Assert.Equal(nameof(MemoryContext), exception.ObjectName);
        }

        [Fact]
        void PAllocReturnsNullPointerForZeroSize()
        {
            using var memoryContext = new MemoryContext();

            var ptr = memoryContext.PAlloc(0ul);

            Assert.Equal(IntPtr.Zero, ptr);
        }

        [Fact]
        void PAllocRecyclesPerfectlySizedChunk()
        {
            using var memoryContext = new MemoryContext();

            var ptr1 = memoryContext.PAlloc(8ul);
            memoryContext.PFree(ptr1);
            var ptr2 = memoryContext.PAlloc(8ul);

            Assert.Equal(ptr1, ptr2);
        }

        [Fact]
        void PAllocRecyclesBiggerChunk()
        {
            using var memoryContext = new MemoryContext();

            var ptr1 = memoryContext.PAlloc(9ul);
            memoryContext.PFree(ptr1);
            var ptr2 = memoryContext.PAlloc(8ul);

            Assert.Equal(ptr1, ptr2);
        }

        [Fact]
        void PAllocPrefersRecyclingOfPerfectlySizedChunk()
        {
            using var memoryContext = new MemoryContext();

            var ptr1 = memoryContext.PAlloc(9ul);
            // We need something between the two candidates.
            // Otherwise PFree will merge them
            memoryContext.PAlloc(1ul);
            var ptr3 = memoryContext.PAlloc(8ul);
            memoryContext.PFree(ptr1);
            memoryContext.PFree(ptr3);
            var ptr4 = memoryContext.PAlloc(8ul);

            Assert.Equal(ptr3, ptr4);
        }

        [Fact]
        void PAllocThrowsWhenOutOfMemory()
        {
            using var memoryContext = new MemoryContext(1ul);

            Assert.Throws<OutOfMemoryException>(() => memoryContext.PAlloc(2UL));
        }

        [Fact]
        void PAllocThrowsWhenOutOfContinuousMemory()
        {
            using var memoryContext = new MemoryContext(3ul);
            var ptr1 = memoryContext.PAlloc(1);
            memoryContext.PAlloc(1);
            var ptr3 = memoryContext.PAlloc(1);
            memoryContext.PFree(ptr1);
            memoryContext.PFree(ptr3);

            Assert.Throws<OutOfMemoryException>(() => memoryContext.PAlloc(2UL));
        }

        [Fact]
        void PAlloc0ZeroesOutMemory()
        {
            const int testValue = 42;
            using var memoryContext = new MemoryContext(4ul);
            var ptr1 = memoryContext.PAlloc(4);
            System.Runtime.InteropServices.Marshal.WriteInt32(ptr1, testValue);
            memoryContext.PFree(ptr1);

            // Just to prove that PFree doesn't change the freed memory
            var memoryContent = System.Runtime.InteropServices.Marshal.ReadInt32(ptr1);
            Assert.Equal(testValue, memoryContent);

            var ptr2 = memoryContext.PAlloc0(4ul);

            // Just to prove that our PFreed memory got recycled
            Assert.Equal(ptr1, ptr2);

            memoryContent = System.Runtime.InteropServices.Marshal.ReadInt32(ptr2);
            Assert.Equal(0, memoryContent);
        }

        [Fact]
        void PAlloc0ThrowsOnDisposedMemoryContext()
        {
            var memoryContext = new MemoryContext();
            memoryContext.Dispose();

            var exception = Assert.Throws<ObjectDisposedException>(() => memoryContext.PAlloc0(8UL));

            Assert.Equal(nameof(MemoryContext), exception.ObjectName);
        }
        
        [Fact]
        void DisposeTwiceSucceeds()
        {
            var memoryContext = new MemoryContext();
            memoryContext.Dispose();
            memoryContext.Dispose();
        }

        [Fact]
        void RePAllocThrowsOnDisposedMemoryContext()
        {
            var memoryContext = new MemoryContext();
            var ptr = memoryContext.PAlloc(4ul);
            memoryContext.Dispose();

            var exception = Assert.Throws<ObjectDisposedException>(() => memoryContext.RePAlloc(ptr,8UL));

            Assert.Equal(nameof(MemoryContext), exception.ObjectName);
        }

        [Fact]
        void RePAllocThrowsOnInvalidPointer()
        {
            using var memoryContext = new MemoryContext();
            var ptr = new IntPtr(123);

            var exception = Assert.Throws<InvalidOperationException>(() => memoryContext.RePAlloc(ptr,8UL));

            Assert.Equal("You can not RePAlloc memory that isn't allocated.", exception.Message);
        }

        [Fact]
        void RePAllocPFreesMemoryOnZeroSize()
        {
            using var memoryContext = new MemoryContext();
            var ptr1 = memoryContext.PAlloc(8ul);
            var ptr2 = memoryContext.RePAlloc(ptr1, 0ul);
            var ptr3 = memoryContext.PAlloc(8ul);

            Assert.NotEqual(IntPtr.Zero, ptr1);
            Assert.Equal(IntPtr.Zero, ptr2);

            // if the PAlloc after RePAlloc returns the same pointer as the PAlloc before it
            // it has been PFreed by RePAlloc and reclaimed by PAlloc
            Assert.Equal(ptr1, ptr3);
        }

        [Fact]
        void RePAllocShrinksMemoryIfNewSizeIsLessThanOldSize()
        {
            using var memoryContext = new MemoryContext(8ul);
            var ptr1 = memoryContext.PAlloc(8ul);

            // Fill the whole buffer with 1 bits
            System.Runtime.InteropServices.Marshal.WriteInt64(ptr1, unchecked((long)ulong.MaxValue));

            var buffer1Value = unchecked((ulong) System.Runtime.InteropServices.Marshal.ReadInt64(ptr1));

            // Shrink the buffer to half of it's original size via RePAlloc
            var ptr2 = memoryContext.RePAlloc(ptr1, 4ul);
            var buffer2Value = unchecked((uint) System.Runtime.InteropServices.Marshal.ReadInt32(ptr2));

            // PAlloc0 the newly freed half to fill it with 0 bits
            var ptr3 = memoryContext.PAlloc0(4ul);
            var buffer3Value = unchecked((uint) System.Runtime.InteropServices.Marshal.ReadInt32(ptr3));

            Assert.Equal(ulong.MaxValue, buffer1Value);

            // We could Assert.Equal(ptr1, ptr2); and Assert.Equal(ptr2 + 4, ptr3);
            // but we don't as this is an implementation detail of the current
            // implementation. The fact that the PAlloc0 of 4 bytes for ptr3 succeeds
            // is enough to prove that RePAlloc shrunk the internal buffer behind ptr1
            // given the fact that our whole buffer only has 8 bytes.
            Assert.Equal(uint.MaxValue, buffer2Value);
            Assert.Equal(0u, buffer3Value);
        }

        [Fact]
        void RePAllocCopiesMemoryIfNewSizeIsEqualToOldSize()
        {
            // In Fact a 8 byte MemoryContext would be enough here since
            // we don't PAlloc in this situation but that's an implementation
            // detail of the current implementation
            using var memoryContext = new MemoryContext(16ul);
            var ptr1 = memoryContext.PAlloc(8ul);

            // Fill the whole buffer with 1 bits
            System.Runtime.InteropServices.Marshal.WriteInt64(ptr1, unchecked((long)ulong.MaxValue));

            var buffer1Value = unchecked((ulong) System.Runtime.InteropServices.Marshal.ReadInt64(ptr1));

            var ptr2 = memoryContext.RePAlloc(ptr1, 8ul);
            var buffer2Value = unchecked((ulong) System.Runtime.InteropServices.Marshal.ReadInt64(ptr2));

            // We could Assert.Equal(ptr1, ptr2); but we don't as this is an
            // implementation detail of the current implementation.
            Assert.Equal(ulong.MaxValue, buffer1Value);
            Assert.Equal(ulong.MaxValue, buffer2Value);
        }

        [Fact]
        void RePAllocCopiesMemoryIfNewSizeIsGreaterThanOldSize()
        {
            using var memoryContext = new MemoryContext(17ul);
            var ptr1 = memoryContext.PAlloc(8ul);

            // Fill the whole buffer with 1 bits
            System.Runtime.InteropServices.Marshal.WriteInt64(ptr1, unchecked((long)ulong.MaxValue));

            var buffer1Value = unchecked((ulong) System.Runtime.InteropServices.Marshal.ReadInt64(ptr1));

            var ptr2 = memoryContext.RePAlloc(ptr1, 9ul);
            var buffer2Value = unchecked((ulong) System.Runtime.InteropServices.Marshal.ReadInt64(ptr2));

            Assert.Equal(ulong.MaxValue, buffer1Value);
            Assert.Equal(ulong.MaxValue, buffer2Value);
        }

        [Fact]
        void PFreeThrowsOnDisposedMemoryContext()
        {
            var memoryContext = new MemoryContext();
            var ptr = memoryContext.PAlloc(4ul);
            memoryContext.Dispose();

            var exception = Assert.Throws<ObjectDisposedException>(() => memoryContext.PFree(ptr));

            Assert.Equal(nameof(MemoryContext), exception.ObjectName);
        }

        [Fact]
        void PFreeToleratesNullPointer()
        {
            using var memoryContext = new MemoryContext();
            memoryContext.PFree(IntPtr.Zero);
        }

        [Fact]
        void PFreeThrowsOnInvalidPointer()
        {
            using var memoryContext = new MemoryContext();
            var ptr = new IntPtr(123);

            var exception = Assert.Throws<InvalidOperationException>(() => memoryContext.PFree(ptr));

            Assert.Equal("You can't free memory that isn't allocated. Are you double freeing it?", exception.Message);
        }

        [Fact]
        void PFreeMergesAdjacentFreeMemoryOnTheLeft()
        {
            using var memoryContext = new MemoryContext(3);
            var ptr1 = memoryContext.PAlloc(1ul);
            var ptr2 = memoryContext.PAlloc(1ul);
            memoryContext.PAlloc(1ul);
            memoryContext.PFree(ptr1);
            memoryContext.PFree(ptr2);

            // No need to assert.
            // If this PAlloc of two bytes succeeds, the
            // merge of free memory on the left has
            // succeeded because otherwise we wouldn't
            // have two adjacent bytes left in the buffer.
            memoryContext.PAlloc(2ul);
        }

        [Fact]
        void PFreeMergesAdjacentFreeMemoryOnTheRight()
        {
            using var memoryContext = new MemoryContext(3);
            memoryContext.PAlloc(1ul);
            var ptr1 = memoryContext.PAlloc(1ul);
            var ptr2 = memoryContext.PAlloc(1ul);
            memoryContext.PFree(ptr2);
            memoryContext.PFree(ptr1);

            // No need to assert.
            // If this PAlloc of two bytes succeeds, the
            // merge of free memory on the right has
            // succeeded because otherwise we wouldn't
            // have two adjacent bytes left in the buffer.
            memoryContext.PAlloc(2ul);
        }

        [Fact]
        void GetPallocFunctionPointerThrowsOnDisposedMemoryContext()
        {
            var memoryContext = new MemoryContext();
            memoryContext.Dispose();

            var exception = Assert.Throws<ObjectDisposedException>(() => memoryContext.GetPAllocFunctionPointer());

            Assert.Equal(nameof(MemoryContext), exception.ObjectName);
        }

        [Fact]
        void GetPallocFunctionPointerReturnsUsableFunctionPointer()
        {
            using var memoryContext = new MemoryContext();

            var pallocPtr = memoryContext.GetPAllocFunctionPointer();
            var pallocDelegate = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PAllocDelegate>(pallocPtr);
            var ptr = pallocDelegate(8ul);
            memoryContext.PFree(ptr);

            Assert.NotEqual(IntPtr.Zero, pallocPtr);
            Assert.NotEqual(IntPtr.Zero, ptr);
            Assert.Equal(memoryContext.PAlloc, pallocDelegate);
            Assert.Equal(8ul, memoryContext.TotalBytesPAlloc);
            Assert.Equal(8ul, memoryContext.TotalBytesPFree);
        }

        [Fact]
        void GetPalloc0FunctionPointerThrowsOnDisposedMemoryContext()
        {
            var memoryContext = new MemoryContext();
            memoryContext.Dispose();

            var exception = Assert.Throws<ObjectDisposedException>(() => memoryContext.GetPAlloc0FunctionPointer());

            Assert.Equal(nameof(MemoryContext), exception.ObjectName);
        }

        [Fact]
        void GetPalloc0FunctionPointerReturnsUsableFunctionPointer()
        {
            using var memoryContext = new MemoryContext();

            var palloc0Ptr = memoryContext.GetPAlloc0FunctionPointer();
            var palloc0Delegate = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PAllocDelegate>(palloc0Ptr);
            var ptr = palloc0Delegate(8ul);
            var bufferContent = System.Runtime.InteropServices.Marshal.ReadInt64(ptr);
            memoryContext.PFree(ptr);

            Assert.NotEqual(IntPtr.Zero, palloc0Ptr);
            Assert.NotEqual(IntPtr.Zero, ptr);
            Assert.Equal(0L, bufferContent);
            Assert.Equal(memoryContext.PAlloc0, palloc0Delegate);
            Assert.Equal(8ul, memoryContext.TotalBytesPAlloc0);
            Assert.Equal(8ul, memoryContext.TotalBytesPFree);
        }

        [Fact]
        void GetRePAllocFunctionPointerThrowsOnDisposedMemoryContext()
        {
            var memoryContext = new MemoryContext();
            memoryContext.Dispose();

            var exception = Assert.Throws<ObjectDisposedException>(() => memoryContext.GetRePAllocFunctionPointer());

            Assert.Equal(nameof(MemoryContext), exception.ObjectName);
        }

        [Fact]
        void GetRePAllocFunctionPointerReturnsUsableFunctionPointer()
        {
            using var memoryContext = new MemoryContext();

            var repallocPtr = memoryContext.GetRePAllocFunctionPointer();
            var repallocDelegate = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<RePAllocDelegate>(repallocPtr);
            var ptr1 = memoryContext.PAlloc(4ul);
            var ptr2 = repallocDelegate(ptr1, 8ul);
            memoryContext.PFree(ptr2);

            Assert.NotEqual(IntPtr.Zero, repallocPtr);
            Assert.NotEqual(IntPtr.Zero, ptr2);
            Assert.Equal(memoryContext.RePAlloc, repallocDelegate);
            Assert.Equal(4ul, memoryContext.TotalBytesPAlloc);
            Assert.Equal(8ul, memoryContext.TotalBytesRePAlloc);
            Assert.Equal(4ul, memoryContext.TotalBytesRePAllocFree);
            Assert.Equal(8ul, memoryContext.TotalBytesPFree);
        }

        [Fact]
        void GetPFreeFunctionPointerThrowsOnDisposedMemoryContext()
        {
            var memoryContext = new MemoryContext();
            memoryContext.Dispose();

            var exception = Assert.Throws<ObjectDisposedException>(() => memoryContext.GetPFreeFunctionPointer());

            Assert.Equal(nameof(MemoryContext), exception.ObjectName);
        }

        [Fact]
        void GetPFreeFunctionPointerReturnsUsableFunctionPointer()
        {
            using var memoryContext = new MemoryContext();

            var pfreePtr = memoryContext.GetPFreeFunctionPointer();
            var pfreeDelegate = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<PFreeDelegate>(pfreePtr);
            var ptr = memoryContext.PAlloc(8ul);
            pfreeDelegate(ptr);
            var exception = Assert.Throws<InvalidOperationException>(() => memoryContext.PFree(ptr));


            Assert.NotEqual(IntPtr.Zero, pfreePtr);
            Assert.NotEqual(IntPtr.Zero, ptr);
            Assert.Equal(memoryContext.PFree, pfreeDelegate);
            Assert.Equal("You can't free memory that isn't allocated. Are you double freeing it?", exception.Message);
            Assert.Equal(8ul, memoryContext.TotalBytesPAlloc);
            Assert.Equal(8ul, memoryContext.TotalBytesPFree);
        }
    }
}
