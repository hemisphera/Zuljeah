# Zuljeah
A live set list organizer and remote control for REAPER

## What is this?

This is a setlist organizer and remote control for the DAW Cockos REAPER (http://reaper.fm).

Zuljeah connects to REAPER over REAPERs Web API.

You define your songs in a single REAPER project, each one a REAPER region. The songs are identified in Zuljeah by the ID that REAPER assignes to these regions. Each region can then be organized into a reorganizable setlist and be triggered in sequence from Zuljeah.

## Configuration
- Set up REAPER: Go to **Preferences** > **Control/OSC/web** and add "Web browser interface" as control surface
- Make note of the access URI that you assign. The URI is usually http://localhost:8080.
- Should you choose a different port than 8080, you need to set that same port in `appsettings.json` (property `ReaperUri`) on Zuljeah

## Usage

Zuljeah has two views. The "Playback" view and the "Setlist Editor" view. The former is the main view you will use for navigatin playback. The latter is an editor that allows you create and edit a setlist.

### Menu

- Player View
  - General
    - **Play**: Starts playback at whatever position the playback cursor in REAPER is at the moment.
    - **Pause**: Toggles pause in REAPER
    - **Stop**: Stops playback in REAPER and moves the playback cursor back to the edit cursor position.
    - **Resynch REAPER**: Should the song regions not show up correctly or not all regions be visible, click this to resynch all regions with REAPER
  - Setlist
    - **Load**: Load an existing setlist from a file
    - **Save**: Save the current setlist to a file
    - **Edit Setlist**: Switch to "Setlist Editor" that allows editing the setlist.
- Setlist Editor View
  - Editor
    - **Delete**: Deletes the setlist entry
    - **Add from REAPER**: Adds all regions that exist in REAPER but don't yet exist in your setlist
    - **Close**: Close the Setlist Editor and return to the player view
