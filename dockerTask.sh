projects=("radio-listener" "radio-scheduler" "radio-identification")

runtimeID="debian.8-x64"
framework="netcoreapp1.1"

# Kills all running containers of an image and then removes them.
cleanAll () {
  docker-compose down --rmi all

  # Remove any dangling images (from previous builds)
  danglingImages=$(docker images -q --filter 'dangling=true')
  if [[ ! -z $danglingImages ]]; then
    docker rmi -f $danglingImages
  fi
}

# Builds the Docker images.
buildImages () {
  echo "Building the project..."
  rm -rf bin
  for project in ${projects[@]}; do
    pubFolder="$(pwd)/bin/release/$framework/publish/${project}"
    cd ${project}
    echo "Publishing to $pubFolder"
    dotnet publish -f $framework -r $runtimeID -c release -o $pubFolder
    cd ..
  done

  echo "Building the image..."
  docker-compose build
}

# Runs docker-compose.
compose () {
  echo "Running compose file..."
  docker-compose kill
  docker-compose up -d
}

# Shows the usage for the script.
showUsage () {
  echo "Usage: dockerTask.sh [COMMAND]"
  echo "    Runs build or compose"
  echo ""
  echo "Commands:"
  echo "    build: Builds all Docker images."
  echo "    compose: Runs docker-compose."
  echo "    clean: Removes the image '$imageName' and kills all containers based on that image."
  echo ""
  echo "Example:"
  echo "    ./dockerTask.sh build"
  echo ""
  echo "    This will:"
  echo "        Build a Docker images."
}

if [ $# -eq 0 ]; then
  showUsage
else
  case "$1" in
    "compose")
            ENVIRONMENT=$(echo $2 | tr "[:upper:]" "[:lower:]")
            compose
            ;;
    "build")
            buildImages
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
