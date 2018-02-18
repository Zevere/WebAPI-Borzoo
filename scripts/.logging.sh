color_green=$(tput setaf 2)
color_normal=$(tput sgr0)

function say {
    if [ $# -ne 1 ]; then
        echo "Incorect number of arguments"
        exit 1
    fi
    printf "\n~~~> ${color_green}$1\n\n${color_normal}"
}
