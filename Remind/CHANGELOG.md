# Changelog

All notable changes to this project will be documented in this file.

## [0.0.1] - 2026-03-12

### Added
- `/remindme` (`/rm`) — show a private notification after a delay (`in hh:mm:ss`) or at a local time (`at HH:mm`)
- `/remindlocal` (`/rl`) — send a local chat message after a delay or at a time
- `/remindglobal` (`/rg`) — send a global chat message after a delay or at a time
- `/remindhelp` (`/rh`) — list all available commands
- `/remindtoggle` (`/rt`) — toggle the mod on/off
- `/remindshowcommand` (`/esc`) — toggle showing commands in chat
- `ScheduledTaskManager` — wall-clock based one-shot task scheduler with `ScheduleIn(TimeSpan)`, `ScheduleAt(DateTime)`, `TryScheduleIn(string, ...)`, and `TryScheduleAt(string, ...)` overloads
