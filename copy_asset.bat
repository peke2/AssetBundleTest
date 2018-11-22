@echo off
robocopy Assets\AssetBundles\json\local Assets\StreamingAssets\local /MIR /xf *.meta
robocopy Assets\AssetBundles\json\remote %TEST_SERVER_PATH%\asset\www\remote /MIR /xf *.meta

pause