// Injector.cpp : Defines the exported functions for the DLL application.
//

#include "Injector.h"


#define WIN32_LEAN_AND_MEAN 
#define CREATE_THREAD_ACCESS (PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ) 

namespace Injector {
	extern "C" __declspec(dllexport) bool Inject() {
		DWORD pID = GetTargetThreadIDFromProcName(L"RocketLeague.exe");

		// Get the dll's full path name 
		wchar_t buf[MAX_PATH] = { 0 };
		GetFullPathNameW(L"RLModding.dll", MAX_PATH, buf, NULL);

		HANDLE Proc;
		HMODULE hLib;
		LPVOID RemoteString, LoadLibAddy;

		if (!pID)
			return false;

		Proc = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pID);
		if (!Proc)
		{
			return false;
		}

		LoadLibAddy = (LPVOID)GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryW");

		// Allocate space in the process for our DLL 
		RemoteString = (LPVOID)VirtualAllocEx(Proc, NULL, sizeof(buf), MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);

		// Write the string name of our DLL in the memory allocated 
		WriteProcessMemory(Proc, (LPVOID)RemoteString, buf, sizeof(buf), NULL);

		// Load our DLL 
		CreateRemoteThread(Proc, NULL, NULL, (LPTHREAD_START_ROUTINE)LoadLibAddy, (LPVOID)RemoteString, NULL, NULL);

		CloseHandle(Proc);
		return true;
	}

	DWORD GetTargetThreadIDFromProcName(const wchar_t * ProcName)
	{
		PROCESSENTRY32W pe;
		HANDLE thSnapShot;
		BOOL retval, ProcFound = false;

		thSnapShot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
		if (thSnapShot == INVALID_HANDLE_VALUE)
		{
			return false;
		}

		pe.dwSize = sizeof(PROCESSENTRY32W);

		retval = Process32FirstW(thSnapShot, &pe);
		while (retval)
		{
			if (StrStrIW(pe.szExeFile, ProcName))
			{
				return pe.th32ProcessID;
			}
			retval = Process32NextW(thSnapShot, &pe);
		}
		return 0;
	}
}
