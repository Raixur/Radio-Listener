imageName="radio-listener"
projectName="radiomonitoring"
serviceName="listener"
containerName="${projectName}_${serviceName}_1"
runtimeID="debian.8-x64"
framework="netcoreapp1.1"

# Setting default environment and compose file.
setDefaults () {
  if [[ -z $ENVIRONMENT ]]; then
    ENVIRONMENT="debug"
  fi

  composeFileName="docker-compose.yml"
  if [[ $ENVIRONMENT != "release" ]]; then
    composeFileName="docker-compose.$ENVIRONMENT.yml"
  fi

  if [[ ! -f $composeFileName ]]; then
    echo "$ENVIRONMENT is not a valid parameter. File '$composeFileName' does not exist."
    exit
  fi
}

# Kills all running containers of an image and then removes them.
cleanAll () {
  setDefaults

  docker-compose -f $composeFileName -p $projectName down --rmi all

  # Remove any dangling images (from previous builds)
  danglingImages=$(docker images -q --filter 'dangling=true')
  if [[ ! -z $danglingImages ]]; then
    docker rmi -f $danglingImages
  fi
}

# Builds the Docker image.
buildImage () {
  setDefaults

  echo "Building the project ($ENVIRONMENT)."
  pubFolder="bin/$ENVIRONMENT/$framework/publish"
  dotnet publish  -f $framework -r $runtimeID -c $ENVIRONMENT -o $pubFolder

  echo "Building the image $imageName ($ENVIRONMENT)."
  docker-compose -f "$pubFolder/$composeFileName" -p $projectName build
}

# Runs docker-compose.
compose () {
  setDefaults

  echo "Running compose file $composeFileName"
  docker-compose -f $composeFileName -p $projectName kill
  docker-compose -f $composeFileName -p $projectName up -d
}

startDebugging () {
  containerId=$(docker ps -f "name=$containerName" -q -n=1)
  if [[ -z $containerId ]]; then
    echo "Could not find a container named $containerName"
  else
    docker exec -i $containerId /clrdbg/clrdbg --interpreter=mi
  fi

}

# Shows the usage for the script.
showUsage () {
  echo "Usage: dockerTask.sh [COMMAND] (ENVIRONMENT)"
  echo "    Runs build or compose using specific environment (if not provided, debug environment is used)"
  echo ""
  echo "Commands:"
  echo "    build: Builds a Docker image ('$imageName')."
  echo "    compose: Runs docker-compose."
  echo "    clean: Removes the image '$imageName' and kills all containers based on that image."
  echo "    composeForDebug: Builds the image and runs docker-compose."
  echo "    startDebugging: Finds the running container and starts the debugger inside of it."
  echo ""
  echo "Environments:"
  echo "    debug: Uses debug environment."
  echo "    release: Uses release environment."
  echo ""
  echo "Example:"
  echo "    ./dockerTask.sh build debug"
  echo ""
  echo "    This will:"
  echo "        Build a Docker image named $imageName using debug environment."
}

if [ $# -eq 0 ]; then
  showUsage
else
  case "$1" in
    "compose")
            ENVIRONMENT=$(echo $2 | tr "[:upper:]" "[:lower:]")
            compose
            ;;
    "composeForDebug")
            ENVIRONMENT=$(echo $2 | tr "[:upper:]" "[:lower:]")
            export REMOTE_DEBUGGING=1
            buildImage
            compose
            ;;
    "startDebugging")
            startDebugging
            ;;
    "build")
            ENVIRONMENT=$(echo $2 | tr "[:upper:]" "[:lower:]")
            buildImage
            ;;
    "clean")
            ENVIRONMENT=$(echo $2 | tr "[:upper:]" "[:lower:]")
            cleanAll
            ;;
    *)
            showUsage
            ;;
  esac
fi
