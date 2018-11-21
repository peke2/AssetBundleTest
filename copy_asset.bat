@echo off
robocopy Assets\AssetBundles\json\local Assets\StreamingAssets\local /MIR
robocopy Assets\AssetBundles\json\remote e:\develop\servers\test\asset\www\remote /MIR

pause