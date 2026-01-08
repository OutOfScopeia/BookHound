#!/bin/bash
tightvncserver :1 &
sleep 0.5
/usr/share/novnc/utils/novnc_proxy --vnc localhost:5901 &
tail -f /dev/null
