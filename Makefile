COMPANY = nodeart
NAME = radiolistener
TAG = $(COMPANY)/$(NAME)

VOLUME_ARGS = -v "$(PWD)/export:/host"
ENV_ARGS = --env-file default.env
SHELL = bash -l
NETWORK = local

TEST_STREAM=http://online-radioroks2.tavrmedia.ua/RadioROKS_Beatles

-include ../Makefile.inc

.PHONY: all build test release shell run start stop rm rmi default

all: build

build:
	@docker build -t $(TAG) --force-rm .

rebuild:
	@docker build -t $(TAG) --force-rm --no-cache .
    
shell:
	@docker exec -it $(NAME) $(SHELL)

run:
	@docker run -it --rm --name $(NAME) $(TAG) $(SHELL) 

launch:
	@docker run -d --name $(NAME) -h $(NAME).local $(ENV_ARGS) $(VOLUME_ARGS) $(TAG)

launch-test:
	@docker run -dt --name $(NAME) $(ENV_ARGS) $(TAG) $(TEST_STREAM)

launch-net:
	@docker run -d --name $(NAME) -h $(NAME).local $(ENV_ARGS) $(VOLUME_ARGS) --network=local --net-alias=$(NAME).local $(TAG)

create-network:
	@docker network create -d bridge local

logs:
	@docker logs $(NAME)

logsf:
	@docker logs -f $(NAME)

start:
	@docker start $(NAME)

kill:
	@docker kill $(NAME)

stop:
	@docker stop $(NAME)

rmf:
	@docker rm -f $(NAME)

rmi:
	@docker rmi $(TAG)

default: build