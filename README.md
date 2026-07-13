About NVMe Monitor
NVMe Monitor is an intelligent, always-on-top Windows overlay designed for PC enthusiasts who demand precision hardware telemetry without the screen clutter. Built with C# and WPF, it interfaces directly with motherboard sensor controllers to provide real-time thermal and health data for your NVMe and SATA storage.

Unlike standard monitors, NVMe Monitor learns your specific hardware configuration and adapts to it.

Key Features
AI-Driven Thermal Configuration: Upon first launch, NVMe Monitor queries Google Gemini to identify the official maximum safe operating temperatures for your specific drives. It saves these limits locally, so your monitor is perfectly tuned for your hardware without manual guesswork.

Dynamic Thermal Thresholds: Every drive in your system is unique. The intuitive Settings UI dynamically generates sliders for every detected drive, allowing you to fine-tune thermal alarm limits in 5% increments in real-time.

Intelligent Health Logging: Beyond just monitoring, the app acts as a diagnostic tool. If a S.M.A.R.T. fault is detected or a critical thermal limit is breached, the app automatically logs the event to your Documents folder, creating a persistent history of drive health.

Unobtrusive Borderless UI: A clean, draggable, and transparent overlay that integrates seamlessly with your desktop. It stays out of your way while gaming or working, but grabs your attention immediately when a drive runs hot.

Visual Alert System: The overlay automatically pulses and highlights specific drives in red if they approach critical temperatures, ensuring you always know the status of your storage at a glance.

Fully Portable: A single, lightweight, self-contained executable. No installers, no registries, no bloat.

Technology Stack
Core: C# / .NET 10 (WPF).

Hardware Interface: LibreHardwareMonitorLib for low-level sensor polling.

AI Integration: Gemini API for intelligent thermal limit discovery.

UI/UX: Native Windows 11 Fluent Design using WPF-UI.

Persistence: Local JSON caching and automated system logging.

Built with help from Google Gemini.
