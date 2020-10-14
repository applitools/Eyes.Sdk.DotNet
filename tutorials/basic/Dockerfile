FROM artemqaapplitools/chrome-docker:latest

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
CMD //chrome_setup.sh > /dev/null 2>&1 && //bootstrap.sh > /dev/null 2>&1 && dotnet test -f netcoreapp2.1 -v n