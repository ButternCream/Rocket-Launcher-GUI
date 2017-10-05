#pragma once
#include <windows.h> 
#include <tlhelp32.h> 
#include <shlwapi.h> 
#pragma comment (lib, "Shlwapi.lib")

namespace Injector {
	extern "C" __declspec(dllexport) bool Inject();
	extern "C" __declspec(dllexport) bool Inject_Beta();
	DWORD GetTargetThreadIDFromProcName(const wchar_t * ProcName);
}