import xml.etree.ElementTree as ET
import os
import sys
import re
import datetime
import json

def upSemanticVersion(currentVersion, part):
    versionPattern = r"(?P<major>[0-9]+)\.(?P<minor>[0-9]+)(\.(?P<patch>[0-9]+))?"
    matches = re.search(versionPattern, currentVersion)
    if matches:
        vMajor = int(matches.group('major'))
        vMinor = int(matches.group('minor'))
        vPatchStr = matches.group('patch')
        if vPatchStr:
            vPatch = int(vPatchStr)
        else:
            vPatch = 0
        vNext = "{}.{}"
        if (part == "major"):
            vNext = vNext.format(vMajor + 1, 0)
        elif (part == "minor"):
            vNext = vNext.format(vMajor, vMinor + 1)
        else:
            vNext = (vNext+".{}").format(vMajor, vMinor, vPatch + 1)
    return vNext

def getDependentPackages(packageName, packageNextVersions, versionPart):
    packageDependecies = {
        "Eyes.Common": ["Eyes.Sdk.Core"],
        "Eyes.Sdk.Core": ["Eyes.Windows", "Eyes.LeanFT", "Eyes.Images", "Eyes.Appium", "Eyes.Selenium"],
        "Eyes.Windows": ["Eyes.CodedUI"],
    }
    dependencyDefaultChangelogStr="### Updated\n- Match to latest {}\n"
    dependentPackages = packageDependecies.get(packageName)
    if dependentPackages:
        for dependentPackageName in dependentPackages:
            if not packageNextVersions.has_key(dependentPackageName):
                packageNextVersions[dependentPackageName] = {
                    "version_part": versionPart, "changelog": dependencyDefaultChangelogStr.format(packageName)
                    }
                getDependentPackages(dependentPackageName, packageNextVersions, versionPart)

def collect_changes():

    packageLastVersions = {}
    packageNextVersions = {}

    originalLines = ""

    f = open("CHANGELOG.md", "r")
    lastVNextPackageName = False
    vNextPattern = r"^## \[(?P<PackageName>.*)? vNext( (?P<VersionPart>.*))?\]$"
    vLastPattern = r"^## \[(?P<PackageName>.*)? (?P<Version>(([0-9]+)\.?)+)\] - .*$"
    for line in f:
        matches = re.search(vNextPattern, line)
        if matches:
            packageName = matches.group('PackageName')
            versionPart = matches.group('VersionPart')
            if versionPart is None:
                versionPart = "minor"
            lastVNextPackageName = packageName
            packageNextVersions[packageName] = {
                "version_part": versionPart, "changelog": ""}
            getDependentPackages(packageName, packageNextVersions, versionPart)
        else:
            matches = re.search(vLastPattern, line)
            if matches:
                originalLines += line
                lastVNextPackageName = None
                packageName = matches.group('PackageName')
                version = matches.group('Version')
                if not packageLastVersions.has_key(packageName):
                    packageLastVersions[packageName] = version
            else:
                if lastVNextPackageName:
                    if (packageNextVersions.has_key(lastVNextPackageName)) and (line != "\n"):
                        packageNextVersions[lastVNextPackageName]["changelog"] += line
                else:
                    originalLines += line

    f.close()

    for (p, v) in packageNextVersions.items():
        vLast = packageLastVersions[p]
        vNext = upSemanticVersion(vLast, v["version_part"])
        v["version"] = vNext
        packageNextVersions[p] = v
    
    return {"next": packageNextVersions, "orig": originalLines}

def update_csproj(name, version_data, release_notes_data):
    xmlfile = name+".DotNet/"+name+".DotNet.csproj"
    tree = ET.parse(xmlfile)
    root = tree.getroot()
    release_notes = root.find('.//PackageReleaseNotes')
    version = root.find('.//Version')
    if release_notes is None:
        print("Could not find the PackageReleaseNotes section in " + xmlfile + "!")
        exit(1)
    if version is None:
        print("Could not find the Version section in " + xmlfile + "!")
        exit(2)
    release_notes.text = release_notes_data
    version.text = version_data
    tree.write(xmlfile)
    print(xmlfile + ": PackageReleaseNotes and Version update done!")

def create_send_mail_json(reported_version, recent_changes):
    f = open("testCoverageGap.txt", "r")
    testCoverageGap = f.read()
    f.close()

    sendMailObj = {
		"sdk": "dotnet",
		"version": reported_version,
		"changeLog": recent_changes,
		"testCoverageGap": testCoverageGap
	}

    specificRecipient = os.environ.get('SPECIFIC_RECIPIENT')
    if specificRecipient is not None:
        sendMailObj["specificRecipient"] = specificRecipient

    return json.dumps(sendMailObj)

if __name__ == '__main__':
    data = collect_changes()
    packageNextVersions = data["next"]
    originalLines = data["orig"]
    newLines = ""
    updated_projects = []
    reported_version = "RELEASE_CANDIDATE"
    dateStr = datetime.datetime.now().strftime("%Y-%m-%d")

    for (p,v) in packageNextVersions.items():
        newLines += "## [" + p + " " + v['version'] + "] - " + dateStr + "\n"
        newLines += v['changelog'] + "\n"
        update_csproj(p, v['version'], v['changelog'])
        updated_projects.append(p + "@" + v['version'] + "\n")
        reported_version += ";" + p + "@" + v['version']
    
    f = open("RECENT_CHANGES.md", "w")
    f.write(newLines)
    f.close()

    sendMailStr = create_send_mail_json(reported_version, newLines);
    f = open("SEND_MAIL.json", "w")
    f.writelines(sendMailStr)
    f.close()

    newLines += originalLines
    
    f = open("CHANGELOG.md", "w")
    f.write(newLines)
    f.close()

    f = open("NEW_TAGS.txt", "w")
    f.writelines(updated_projects)
    f.close()

    os.environ["NEW_TAGS"] = reported_version