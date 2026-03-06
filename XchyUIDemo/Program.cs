using XchyUI.GLFW.window;
using XchyUIDemo;

HotkeyManager.Start();
WindowManager.Get().Init();
WindowManager.Get().SetMainWindow(new MainWindow());
WindowManager.Get().Start();
