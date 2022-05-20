#!/bin/bash

echo "Instalador do programa CS iniciado"
sudo mkdir /opt/conect-server-tools/
sudo cp -r ./* /opt/conect-server-tools
cd /opt/conect-server-tools
sudo ln -s /opt/conect-server-tools/config/cs_completion  /etc/bash_completion.d/cs_completion
cd $HOME/.local/bin
ln -s /opt/conect-server-tools/cs cs
echo "Instalador Finalizado"