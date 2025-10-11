@echo off
cd ..\gymappforbigmuscle\frontend

start "" cmd /k "yarn dev"


start "" "C:\Users\tenye\AppData\Local\Programs\Opera GX\opera.exe" --new-window "https://localhost:7226/swagger" "http://localhost:5174/"
