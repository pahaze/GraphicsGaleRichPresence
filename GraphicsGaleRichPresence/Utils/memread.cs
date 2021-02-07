using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

namespace GraphicsGaleRichPresence.Utils
{
    // memory reading class, used to well... read memory of an application
    class memreadApi
    {
        // constants information can be found in <winnt.h>
        [Flags]
        public enum ProcessAccessType
        {
            PROCESS_TERMINATE = (0x0001),
            PROCESS_CREATE_THREAD = (0x0002),
            PROCESS_SET_SESSIONID = (0x0004),
            PROCESS_VM_OPERATION = (0x0008),
            PROCESS_VM_READ = (0x0010),
            PROCESS_VM_WRITE = (0x0020),
            PROCESS_DUP_HANDLE = (0x0040),
            PROCESS_CREATE_PROCESS = (0x0080),
            PROCESS_SET_QUOTA = (0x0100),
            PROCESS_SET_INFORMATION = (0x0200),
            PROCESS_QUERY_INFORMATION = (0x0400),
            PROCESS_QUERY_LIMITED_INFORMATION = (0x1000)
        }

        // function declarations are found in the MSDN and in <winbase.h> 

        //      HANDLE OpenProcess(
        //          DWORD dwDesiredAccess,  // access flag
        //          BOOL bInheritHandle,    // handle inheritance option
        //          DWORD dwProcessId       // process identifier
        //          );
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        //      BOOL CloseHandle(
        //          HANDLE hObject   // handle to object
        //          );
        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hObject);

        //      BOOL ReadProcessMemory(
        //          HANDLE hProcess,              // handle to the process
        //          LPCVOID lpBaseAddress,        // base of memory area
        //          LPVOID lpBuffer,              // data buffer
        //          SIZE_T nSize,                 // number of bytes to read
        //          SIZE_T * lpNumberOfBytesRead  // number of bytes read
        //          );
        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);

        //      BOOL WriteProcessMemory(
        //          HANDLE hProcess,                // handle to process
        //          LPVOID lpBaseAddress,           // base of memory area
        //          LPCVOID lpBuffer,               // data buffer
        //          SIZE_T nSize,                   // count of bytes to write
        //          SIZE_T * lpNumberOfBytesWritten // count of bytes written
        //          );
        [DllImport("kernel32.dll")]
        public static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesWritten);
    }

    class memread
    {
        public Process ReadProcess { get; set; }

        private IntPtr handle;

        public void OpenProcess()
        {
            memreadApi.ProcessAccessType access = memreadApi.ProcessAccessType.PROCESS_QUERY_INFORMATION |
                memreadApi.ProcessAccessType.PROCESS_VM_READ |
                memreadApi.ProcessAccessType.PROCESS_VM_WRITE |
                memreadApi.ProcessAccessType.PROCESS_VM_OPERATION;
            handle = memreadApi.OpenProcess((uint)access, 1, (uint)ReadProcess.Id);
        }

        public void CloseHandle()
        {
            int returnvalue = memreadApi.CloseHandle(handle);
            if (returnvalue != 0)
            {
                throw new Exception("An error occurred while closing the handle.");
            }
        }

        public byte[] ReadMemory(IntPtr memoryAddress, uint bytestoread, out int bytesRead)
        {
            byte[] buffer = new byte[bytestoread];
            IntPtr pointerbytesRead = IntPtr.Zero;
            memreadApi.ReadProcessMemory(handle, memoryAddress, buffer, bytestoread, out pointerbytesRead);
            bytesRead = pointerbytesRead.ToInt32();
            return buffer;
        }

        public void WriteMemory(IntPtr memoryAddress, byte[] buffer, out int bytesWritten)
        {
            IntPtr pointerbytesWritten = IntPtr.Zero;
            memreadApi.WriteProcessMemory(handle, memoryAddress, buffer, (uint)buffer.Length, out pointerbytesWritten);
            bytesWritten = pointerbytesWritten.ToInt32();
        }
    }
}
