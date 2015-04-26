@echo off
rem Clean
set folder="%1"
CMD /C npm install -g rimraf
echo "cleaning %folder%"
CMD /C rimraf %folder%
echo "rimraf completed"