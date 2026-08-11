@echo off
setlocal
set "CFG=%~dp0..\config\kof2002-netplay-low-latency.cfg"
(
echo # Rescue netplay profile for unstable Radmin VPN links.
echo netplay_delay_frames = "60"
echo netplay_check_frames = "0"
echo netplay_client_swap_input = "true"
echo netplay_spectator_mode_enable = "false"
echo video_max_swapchain_images = "3"
echo video_hard_sync = "false"
echo video_hard_sync_frames = "0"
echo video_frame_delay = "0"
echo video_threaded = "false"
echo audio_latency = "96"
echo run_ahead_enabled = "false"
) > "%CFG%"
echo Switched to rescue profile. Restart netplay to apply.
pause
