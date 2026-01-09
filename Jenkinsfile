pipeline {
  agent any

  options {
    timestamps()
  }

  parameters {
    string(name: 'PACKAGE_VERSION', defaultValue: '', description: 'Optional override for NuGet package version (e.g., 1.2.3).')
    string(name: 'NEXUS_NUGET_SOURCE', defaultValue: 'https://nexus.example.com/repository/nuget-hosted/index.json', description: 'Nexus NuGet feed URL.')
  }

  environment {
    DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    NUGET_XMLDOC_MODE = 'skip'
    NEXUS_API_KEY = credentials('nexus-nuget-api-key')
  }

  stages {
    stage('Restore') {
      steps {
        sh 'dotnet restore RuleEngine.sln'
      }
    }

    stage('Build') {
      steps {
        sh 'dotnet build RuleEngine.sln -c Release --no-restore'
      }
    }

    stage('Pack') {
      steps {
        sh '''
          rm -rf artifacts
          mkdir -p artifacts

          VERSION_ARG=""
          if [ -n "${PACKAGE_VERSION}" ]; then
            VERSION_ARG="-p:PackageVersion=${PACKAGE_VERSION}"
          fi

          dotnet pack src/RuleEngine.Core/RuleEngine.Core.csproj -c Release --no-build -o artifacts ${VERSION_ARG}
          dotnet pack src/RuleEngine.Sqlite/RuleEngine.Sqlite.csproj -c Release --no-build -o artifacts ${VERSION_ARG}
          dotnet pack src/CampaignEngine.Core/CampaignEngine.Core.csproj -c Release --no-build -o artifacts ${VERSION_ARG}

          dotnet pack demo/RuleEngineDemoVue/RuleEngineDemoVue.Server/RuleEngineDemoVue.Server.csproj \
            -c Release --no-build -o artifacts ${VERSION_ARG} \
            -p:IsPackable=true \
            -p:BuildProjectReferences=false
        '''
      }
    }

    stage('Publish to Nexus') {
      steps {
        sh '''
          if [ -z "${NEXUS_API_KEY}" ]; then
            echo "NEXUS_API_KEY is not set. Skipping publish."
            exit 0
          fi

          if [ -z "${NEXUS_NUGET_SOURCE}" ]; then
            echo "NEXUS_NUGET_SOURCE is not set. Skipping publish."
            exit 0
          fi

          for pkg in artifacts/*.nupkg; do
            if [ -f "$pkg" ]; then
              dotnet nuget push "$pkg" \
                --api-key "${NEXUS_API_KEY}" \
                --source "${NEXUS_NUGET_SOURCE}" \
                --skip-duplicate
            fi
          done
        '''
      }
    }
  }
}
