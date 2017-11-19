# AWS Ubuntu 16.04 Setup

Get key

```bash
chmod 400 ~/keys/zevere-key.pem
ssh -i ~/keys/zevere-key.pem ubuntu@192.0.2.0

# set alias for easy access
echo 'alias ssh-zv-dev="ssh -i ~/keys/zevere-key.pem ubuntu@192.0.2.0"' >> ~/.bashrc

. ~/.bashrc
```

Initial setup

```bash
# fix locale
sudo apt-get install language-pack-en

# set alias
echo 'alias updg="sudo apt-get update && sudo apt-get upgrade -y && sudo apt-get dist-upgrade -y && sudo apt-get autoremove -y"' >> ~/.bashrc

. ~/.bashrc

# upgrade
updg

# open ports on firewall
sudo ufw allow ssh
sudo ufw allow http
sudo ufw allow https
sudo ufw allow 2376 # Docker
sudo ufw enable

# set hostname, if applicable
sudo vi /etc/hostname
sudo reboot
```

// ToDo: Add a new user

Docker

```bash
# install
sudo apt-get install docker.io -y

# add current user to docker group
sudo adduser $(users) docker

# logout and login again
```

In order to generate certs, follow [this gist](https://gist.github.com/pouladpld/30b08be63f065ee81f7231b492c738fa).
