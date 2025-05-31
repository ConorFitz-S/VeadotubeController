# Veadtube controller!
This is a unity app, which reads a value from a memory address of a given process, and controls Veadotube Mini states with it!

![image](https://github.com/user-attachments/assets/37849dec-9a47-4344-907d-91eb58613f0b)

## RECOMMENDED:
- Have veadotube mini open before launching this app
- Click "Connect Process" only once the desired process is running

### GETTING STARTED:
- Launch the app after starting up veadotube mini.
- Note the state names in your veadotube mini instance.
- Click on the "+" button to add a handle.

![image](https://github.com/user-attachments/assets/dcccf3ec-a1a0-49c1-abdf-429dd5c4b1ad)

- This is a handle. The number on the top is the Handle Value. You can click and drag the handle across the slider bar to change its value.
- If the Current Value is greater than the Handle Value, the app will set the veadotube state to whatever is in the inputfield.
- In the below example, if the current value is greater than 60 but less than 80, it will set the state to "deer". However if the current value is greater than 80, it will set the state to "frog"

![image](https://github.com/user-attachments/assets/7e881653-b9f5-4d98-b770-1af4399c3e9a)

- Click the "x" button at the top right of a handle to delete it.

Keep in mind. all settings are saved. This includes handles, their state names, and positions/values.

## Preview mode

Click the "Preview mode" toggle to activate preview mode. This creates a Preview Handle on the bar, which simulates the value being read from the chosen game by this app.

![image](https://github.com/user-attachments/assets/d947f804-6615-4e18-9ef1-5a8023c82e3c)

This lets you test the functionality of the app before hooking it into a game.
Keep in mind, only the preview value is considered with preview mode enabled. The current value being read from the game will not be assessed while preview mode is enabled.
