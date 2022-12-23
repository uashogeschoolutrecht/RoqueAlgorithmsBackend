# High preformance Cloud setup for a docker enviroment.
This guide expects you to have the workspaces needed already running. 
And work with `Visual studio code` now called `VSC`.
The directions start at `https://portal.live.surfresearchcloud.nl/`

## Requirements
Make sure you have installed the `remote - SSH` extension.


## Your profile
Here are a few things you need to setup.
On your profile your can see your username, copy or remember it! It will be alot easier.

1. In the navigation bar click on `Profile`.
2. Once you are on your profile page click on the person icon with the gear, in the top right of your profile card. 
3. Then a list of options popup, you need to click the one regarding the **time based password**.
4. Using an authenticator app set up the time based password.
5. After that you need to set up your **public ssh key**. 
   1. In a terminal enter the command `ssh-keygen`.
   2. After that you will get a prompt to where to store the keys, the default is fine, then press enter.
   3. After that you need to enter a password.
6. When you have generated your ssh keys, go to the location you have given.
7. Open the file ending in `.pub` and copy the content.
8. Complete step 1 and 2, then click on the `change your profile` link.
9. Put your ssh key in the text box under `SSH public keys`.
10. Click on `Update`.
11. To check if your ssh key was successfully added complete step 1 and 2 again, but then click on the `Show you ssh key(s)` button. If is was successful your key will show up.

## Docker environment
1. In `VSC` click in the bottom left corner of the screen where the (`><` looks like) is.
2. Then a menu pops up then click on `Open ssh configuration file`.
3. Then click on the link ending with `.ssh\config`
4. Then fill in and insert: 
```
Host <Name of your ssh connection>
   HostName <IP adres of the running workspace>
   User <username of surf account>
   ```
5. Complete step 1 again and the click on `Connect to Host` 
6. Click on the name of your ssh connection.
7. Input your given password for the ssh key.

