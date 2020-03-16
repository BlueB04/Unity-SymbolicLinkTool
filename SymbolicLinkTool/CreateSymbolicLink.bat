@echo off
chcp 65001
setlocal
if %1==/D (
    set isdir=1
)
if %isdir%==1 (
    mklink %1 %2 %3
) else (
    mklink %1 %2
)

endlocal
chcp 932