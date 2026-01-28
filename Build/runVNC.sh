#!/bin/bash
/usr/share/novnc/utils/novnc_proxy --listen 6080 --vnc localhost:5901 &
exec vncserver :1 -geometry 2560x1440 -localhost -SecurityTypes None -fg
