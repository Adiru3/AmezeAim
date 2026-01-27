# AmazeAim 🎯

**AmazeAim** is a lightweight system utility designed for competitive gamers to minimize mouse input lag and stabilize polling rate consistency on Windows. By optimizing CPU scheduling, power delivery, and process priorities, it ensures that your mouse data reaches your game with the lowest possible latency.

---

## 🚀 Key Features

* **Process Priority & Affinity:** Automatically detects mouse drivers (LGHUB, Razer, SteelSeries, etc.) and sets them to `High Priority`, pinning them to specific CPU cores (bypassing Core 0) to avoid "starvation" from heavy game engines.
* **System Responsiveness:** Tweaks `Win32PrioritySeparation` and `SystemResponsiveness` to favor short, high-speed input tasks.
* **USB Power Optimization:** Forces USB ports to stay in high-performance mode, preventing "Selective Suspend" which causes micro-stutters.
* **BCDEDIT Tweaks:** Disables `Dynamic Tick` and forces `Platform Tick` (HPET) for stable, jitter-free timing.
* **Debug Console:** Real-time logging to monitor which optimizations are active and see driver boosting in action.
* **Stealth Mode:** Runs silently in the System Tray with minimal CPU and RAM footprint.

---

## 🛠️ How it works

AmazeAim applies a combination of registry edits, system command executions, and active background monitoring:

1.  **Registry Level:** Adjusts the Windows Scheduler to prioritize input.
2.  **Power Level:** Modifies the current Power Plan for maximum USB performance.
3.  **Process Level:** Every 30 seconds, it ensures your mouse software is running on the most efficient CPU cores.

---

## 📥 Installation

1.  Download the latest `AmazeAim.exe` (or compile it from source).
2.  Run as **Administrator** (required to modify system registry and task scheduler).
3.  The app will automatically:
    * Copy itself to `C:\Program Files\AmazeAim`.
    * Create a Task Scheduler entry to start with Windows at Highest Privileges.
    * Initialize background monitoring in the system tray.

---

## 💻 Compilation

If you want to build it yourself using the built-in .NET compiler:

```cmd
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:AmazeAim.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll AmazeAim.cs
