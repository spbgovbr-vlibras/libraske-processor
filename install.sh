#!/usr/bin/env bash

LINUX_PACKAGES="build-essential python3-pip python3-opencv"

function install_system_dependencies {
  echo "Installing required system dependencies..."
  (apt --assume-yes install $LINUX_PACKAGES) || return 1
  return 0
}

function install_python_packages {
  echo -e "\nInstalling required python packages..."
  (pip install -r requirements.txt) || return 1
  return 0
}

install_system_dependencies &&\
install_python_packages &&\

echo -e "\nSuccessful installation!" || echo -e "\nInstallation failed!"
