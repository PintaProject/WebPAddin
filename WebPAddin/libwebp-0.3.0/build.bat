call "C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\vcvarsall.bat" x86
nmake /f Makefile.vc CFG=release-dynamic RTLIBCFG=static OBJDIR=output
xcopy output\release-dynamic\x86\bin\libwebp.dll ..\lib
pause
