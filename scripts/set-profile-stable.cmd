@echo off
setlocal
set "CFG=%~dp0..\config\kof2002-netplay-low-latency.cfg"
(
echo # Stable netplay profile for moderate jitter.
echo netplay_delay_frames = "24"
echo netplay_check_frames = "0"
echo netplay_client_swap_input = "true"
echo netplay_spectator_mode_enable = "false"
echo video_max_swapchain_images = "2"
echo video_hard_sync = "true"
echo video_hard_sync_frames = "0"
echo video_frame_delay = "0"
echo video_threaded = "false"
echo audio_latency = "64"
echo run_ahead_enabled = "false"
) > "%CFG%"
echo Switched to stable profile. Restart netplay to apply.
pause
