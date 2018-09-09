# Ubuntu 16.04 Setup

## Initial server setup

### Create admin user

```bash
sudo useradd myusername --user-group --groups sudo --create-home --shell /bin/bash

# change password if required:
# sudo passwd myusername
```

#### [OPTIONAL] Run sudo w/o password

```bash
sudo visudo

# Allow members of group sudo to execute any command
# %sudo   ALL=(ALL:ALL) NOPASSWD:ALL
```

#### [OPTIONAL] Use vi editor

```bash
sudo apt install -y vim
sudo update-alternatives --config editor
# select vim from the menu
```

### [OPTIONAL] Fix locale

```bash
# fix locale
sudo apt-get install language-pack-en
```

### Access via SSH

#### On client machince

```bash
# create SSH key
ssh-keygen -t rsa
# assuming pub key is store at ~/.ssh/id_rsa_zv_borzoo.pub

# copy SSH pub key to server
ssh-copy-id -i ~/.ssh/id_rsa_zv_borzoo.pub myusername@192.0.2.0

# Connect to server
ssh -i ~/.ssh/id_rsa_zv_borzoo myusername@192.0.2.0

# [OPTIONAL] set alias to connect
echo 'alias ssh-zv-borzoo="ssh -i ~/.ssh/id_rsa_zv_borzoo myusername@192.0.2.0"' >> ~/.bashrc
```

### Upgrade apps

```bash
# set alias
echo 'alias updg="sudo apt-get update && sudo apt-get upgrade -y && sudo apt-get dist-upgrade -y && sudo apt-get autoremove -y"' >> ~/.bashrc

. ~/.bashrc

# upgrade
updg
```

### Open ports

```bash
sudo ufw allow ssh
sudo ufw allow http
sudo ufw allow https
sudo ufw allow 2376 # Docker
sudo ufw enable
```

### [OPTIONAL] Configure hostname

```bash
# set hostname, if applicable
sudo vi /etc/hostname
sudo reboot
```

## Docker

```bash
# install
sudo apt-get install docker.io -y
sudo systemctl enable docker

# add current user to docker group
sudo adduser $(whoami) docker

# logout and login again

# check setup
docker ps
```

## DNS setup

> Skip this part if you already have a DNS name for the server.

### Run web server

Some DNS services require a web server running on the host.

```bash
# Run nginx web server
docker run -d --name nginx --restart always --publish 80:80 nginx
```

### Register IP with DNS service

You could use [freenom](http://www.freenom.com).

## Configure Docker remote connection

In order to generate certs, follow [this gist](https://gist.github.com/pouladpld/30b08be63f065ee81f7231b492c738fa).
