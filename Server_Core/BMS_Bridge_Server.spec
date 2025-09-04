# -*- mode: python ; coding: utf-8 -*-

# This is the "recipe" file for PyInstaller.
# It tells PyInstaller how to bundle the Python server into a single .exe file.

a = Analysis(
    ['main.py'],
    pathex=['.'],
    binaries=[],
    # --- ИСПРАВЛЕНИЕ ЗДЕСЬ ---
    # Мы добавляем все папки, которые нужны серверу для работы.
    # Формат: ('исходная_папка', 'папка_назначения_в_сборке')
    datas=[
        ('falcon_memreader.py', '.'),
        ('config/settings.json', 'config'),
        ('templates', 'templates'),
        ('static', 'static'),
        ('libs', 'libs'),
        ('procedure', 'procedure')
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
    console=True
)