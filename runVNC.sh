#!/bin/bash
tightvncserver :1
sleep 0.5
/usr/share/novnc/utils/novnc_proxy --listen 6080 --vnc localhost:5901 &
tail -f /dev/null
