using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace PlClr.Managed.Tests
{
    public class MemoryContext : IDisposable
    {
        private readonly ulong _size;
        private readonly IntPtr _baseAddress;
        private IntPtr _currentAddress;
        private readonly Dictionary<IntPtr, ulong> _allocatedChunks = new Dictionary<IntPtr, ulong>();
        private readonly Dictionary<IntPtr, ulong> _freedChunks = new Dictionary<IntPtr, ulong>();
        private bool _disposed;

        public MemoryContext(ulong size = 8192UL)
        {
            _size = size;
            _baseAddress = System.Runtime.InteropServices.Marshal.AllocCoTaskMem((int)size);
            _currentAddress = _baseAddress;
        }

        public ulong TotalBytesPAlloc0 { get; private set; }
        public ulong TotalBytesPAlloc { get; private set; }
        public ulong TotalBytesPFree { get; private set; }
        public ulong TotalBytesRePAlloc { get; private set; }
        public ulong TotalBytesRePAllocFree { get; private set; }

        public IntPtr PAlloc(ulong size)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MemoryContext));

            if (size == 0)
                return IntPtr.Zero;

            var retVal = IntPtr.Zero;
            var nextBestChunkKey = IntPtr.Zero;

            // First we look if we can recycle some memory
            foreach (var freedChunk in _freedChunks)
            {
                // If we have exactly the size we're looking for that's perfect
                // we just recycle it.
                if (freedChunk.Value == size)
                {
                    _allocatedChunks.Add(freedChunk.Key, size);
                    _freedChunks.Remove(freedChunk.Key);
                    retVal =  freedChunk.Key;
                    break;
                }
                // If we have a chunk that is bigger than the requested chunk
                // we mark it but we keep on looking for a perfect match
                if (freedChunk.Value > size)
                {
                    nextBestChunkKey = freedChunk.Key;
                }
            }

            if (retVal == IntPtr.Zero)
            {
                if (nextBestChunkKey != IntPtr.Zero)
                {
                    var newSize = _freedChunks[nextBestChunkKey] - size;
                    var newAddress = nextBestChunkKey + (int)size;
                    _allocatedChunks.Add(nextBestChunkKey, size);
                    _freedChunks.Remove(nextBestChunkKey);
                    _freedChunks.Add(newAddress, newSize);
                    retVal = nextBestChunkKey;
                }
                else if (BytesLeft < size)
                {
                    TotalBytesPAlloc -= size;
                    throw new OutOfMemoryException();
                }
                else
                {
                    _allocatedChunks.Add(_currentAddress, size);
                    retVal = _currentAddress;
                    _currentAddress += (int)size;
                }
            }

            TotalBytesPAlloc += size;
            return retVal;
        }

        public unsafe IntPtr PAlloc0(ulong size)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MemoryContext));
 
            var ptr = PAlloc(size);
            var buffer = new Span<byte>((void*)ptr, (int)size);
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }

            TotalBytesPAlloc -= size;
            TotalBytesPAlloc0 += size;
            return ptr;
        }

        public unsafe IntPtr RePAlloc(IntPtr oldPtr, ulong newSize)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MemoryContext));
 
            if (!_allocatedChunks.ContainsKey(oldPtr))
                throw new InvalidOperationException(
                    "You can not RePAlloc memory that isn't allocated.");

            IntPtr newPtr;
            var oldSize = _allocatedChunks[oldPtr];
            if (newSize > 0)
            {
                if (newSize < oldSize)
                {
                    var freedSize = oldSize - newSize;
                    _allocatedChunks[oldPtr] = newSize;
                    _freedChunks[oldPtr + (int) newSize] = freedSize;
                    newPtr = oldPtr;
                }
                else if (newSize == oldSize)
                {
                    newPtr = oldPtr;
                }
                else
                {
                    newPtr = PAlloc(newSize);
                    TotalBytesPAlloc -= newSize;
                    new ReadOnlySpan<byte>((void*)oldPtr, (int)oldSize).CopyTo(new Span<byte>((void*)newPtr, (int)newSize));
                    PFree(oldPtr);
                    TotalBytesPFree -= oldSize;
                }
            }
            else
            {
                newPtr = IntPtr.Zero;
                PFree(oldPtr);
                TotalBytesPFree -= oldSize;
            }

            TotalBytesRePAlloc += newSize;
            TotalBytesRePAllocFree += oldSize;
            return newPtr;
        }

        public void PFree(IntPtr ptr)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MemoryContext));

            if (ptr == IntPtr.Zero)
                return;

            if (!_allocatedChunks.ContainsKey(ptr))
                throw new InvalidOperationException(
                    "You can't free memory that isn't allocated. Are you double freeing it?");

            var size = _allocatedChunks[ptr];
            TotalBytesPFree += size;
            _allocatedChunks.Remove(ptr);

            var newSize = size;
            var merged = false;

            RESTART:
            // First try to merge the memory with another chunk
            foreach (var freedChunk in _freedChunks)
            {

                // if there is an adjacent free chunk right of us
                // we remove it and add it's size to our size.
                if (ptr + (int)newSize == freedChunk.Key)
                {
                    newSize = size + freedChunk.Value;
                    _freedChunks.Add(ptr, newSize);
                    _freedChunks.Remove(freedChunk.Key);
                    merged = true;
                    goto RESTART;
                }

                // if there is an adjacent free chunk left of us
                // we simply add our size to it's size.
                if (freedChunk.Key + (int)freedChunk.Value == ptr)
                {
                    _freedChunks[freedChunk.Key] = freedChunk.Value + newSize;
                    merged = true;
                    goto RESTART;
                }
            }

            if (!merged)
                _freedChunks.Add(ptr, size);
        }

        public IntPtr GetPAllocFunctionPointer(PAllocDelegate? pAllocDelegate = null)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MemoryContext));
 
            return System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(pAllocDelegate ?? PAlloc);
        }

        public IntPtr GetPAlloc0FunctionPointer(PAllocDelegate? pAlloc0Delegate = null)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MemoryContext));
 
            return System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(pAlloc0Delegate ?? PAlloc0);
        }

        public IntPtr GetRePAllocFunctionPointer(RePAllocDelegate? rePAllocDelegate = null)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MemoryContext));
 
            return System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(rePAllocDelegate ?? RePAlloc);
        }

        public IntPtr GetPFreeFunctionPointer(PFreeDelegate? pFreeDelegate = null)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MemoryContext));
 
            return System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(pFreeDelegate ?? PFree);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(_baseAddress);
            _disposed = true;
        }

        private ulong BytesLeft => _size - ((ulong)_currentAddress.ToInt64() - (ulong)_baseAddress.ToInt64());
    }
}
