# -*- mode: python ; coding: utf-8 -*-

# This is the "recipe" file for PyInstaller.
# It tells PyInstaller how to bundle the Python server into a single .exe file.

a = Analysis(
    ['main.py'],
    # --- KEY CHANGE: Tell PyInstaller to look for modules in the current folder ---
    # This is crucial for correctly importing our local modules like 'adapters'.
    pathex=['.'],
    binaries=[],
    # --- Data files to include in the bundle ---
    # We need to explicitly tell PyInstaller to include non-.py files
    # that our application depends on.
    # Format: ('source_path_on_disk', 'destination_folder_in_bundle')
    datas=[
        ('falcon_memreader.py', '.'),
        ('config/settings.json', 'config')
    ],
    hiddenimports=[],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    win_no_prefer_redirects=False,
    win_private_assemblies=False,
    cipher=None,
    noarchive=False,
)
pyz = PYZ(a.pure)

exe = EXE(
    pyz, a.scripts, a.binaries, a.zipfiles, a.datas,
    [], name='BMS_Bridge_Server',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    runtime_tmpdir=None,
    # console=True means the executable will open a command window when run.
    # This is very useful for debugging. To hide it, change to console=False.
    console=True
)