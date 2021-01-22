FROM ubuntu:18.04

RUN apt-get update && apt-get clean && apt-get install -y \
    x11vnc \
    xvfb \
    fluxbox \
    wmctrl \
    wget \
    curl \
    gnupg2 \
    default-jdk \
    unzip \
    && apt update \
    && apt install -y git

RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list \
    && apt-get update && apt-get -y install google-chrome-stable

COPY /tutorials/common/chrome_wrapper.sh /opt/bin/wrap_chrome_binary
RUN /opt/bin/wrap_chrome_binary

RUN wget "https://chromedriver.storage.googleapis.com/$(wget -qO - "https://chromedriver.storage.googleapis.com/LATEST_RELEASE")/chromedriver_linux64.zip"
RUN unzip chromedriver_linux64
RUN mv chromedriver /usr/bin/

RUN mkdir /home/project

ENV DISPLAY :1
COPY /tutorials/common/bootstrap.sh /

RUN wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb  \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y apt-transport-https \
    && apt-get update \
    && apt-get install -y dotnet-sdk-2.1 \
    && apt-get install -y dotnet-sdk-3.1 \
    && apt-get install -y libgdiplus libc6-dev \
    && rm -rf /var/lib/apt/lists/*

ARG repo=tutorial-selenium-csharp-basic
ENV REPOSITORY_NAME=$repo
WORKDIR /home/project/${REPOSITORY_NAME}
RUN git clone https://github.com/applitools/${REPOSITORY_NAME}.git . \
    && sed -i 's/"APPLITOOLS_API_KEY"/Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY")/g' ./ApplitoolsTutorial/BasicDemo.cs \
    && sed -i 's-<PackageReference Include="Eyes.Selenium".*-<ProjectReference Include="/SDK/Eyes.Selenium.DotNet/Eyes.Selenium.DotNet.csproj"/>-g' ./ApplitoolsTutorial/ApplitoolsTutorial.csproj 

COPY . /SDK/
CMD //bootstrap.sh > /dev/null 2>&1 && dotnet test -f netcoreapp2.1