@echo off
rem Clean
set folder="%1"
CMD /C npm install -g rimraf
echo "cleaning %folder%"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (
  @if not exist %%~Ni.bat (CMD /C rimraf "%%i")
)
echo "rimraf completed"
