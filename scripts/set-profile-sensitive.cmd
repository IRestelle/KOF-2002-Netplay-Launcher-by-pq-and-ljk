@echo off
setlocal
set "CFG=%~dp0..\config\kof2002-netplay-low-latency.cfg"
(
echo # Sensitive netplay profile for stable low-latency links.
echo netplay_delay_frames = "1"
echo netplay_check_frames = "0"
echo netplay_client_swap_input = "true"
echo netplay_spectator_mode_enable = "false"
echo video_max_swapchain_images = "2"
echo video_hard_sync = "true"
echo video_hard_sync_frames = "0"
echo video_frame_delay = "6"
echo video_threaded = "false"
echo audio_latency = "32"
echo run_ahead_enabled = "false"
) > "%CFG%"
echo Switched to sensitive profile. Restart netplay to apply.
pause
