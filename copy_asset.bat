@echo off
robocopy Assets\AssetBundles\json\local Assets\StreamingAssets\local /MIR
robocopy Assets\AssetBundles\json\remote %TEST_SERVER_PATH%\asset\www\remote /MIR

pause