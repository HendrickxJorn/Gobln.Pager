# Automatic NuGet publish on push to master

**Date:** 2026-09-01
**Repo:** Gobln.Pager
**Status:** Approved, ready for implementation planning

## Goal

When a commit lands on `master`, build and test the solution, then publish any
package whose `<Version>` is not already on nuget.org. Tag the commit and create
a GitHub Release when a new version actually ships.

## Decisions

| Decision | Choice | Reasoning |
|---|---|---|
| Version source | `<Version>` in each `.csproj` | One line to edit; no tag ceremony; the repo stays the source of truth |
| Publish condition | `dotnet nuget push --skip-duplicate` | No version bump means nothing is uploaded, so every push is safe to run |
| Trigger branch | `master` | `origin/HEAD` points at `master`; a `main` branch also exists on the remote but is not the default |
| Test gate | Publish blocked on test failure | A bad package on nuget.org cannot be deleted, only unlisted |
| Runner | `windows-latest` | `.Mvc` references `System.Web` for `net462`/`net48`; `Gobln.Pager.Mvc48Test` targets `net48` |
| Extras | GitHub Release + tag, `.nupkg` build artifacts | No symbol packages, so `DebugType=none` in Release stays as-is |

## Packages published

All five libraries under `scr/`:

| Project | Current `<Version>` | After this change |
|---|---|---|
| `Gobln.Pager` | 2.0.0 | 2.0.0 (unchanged) |
| `Gobln.Pager.Mvc` | *(none)* | 2.0.0 |
| `Gobln.Pager.Blazor` | *(none)* | 2.0.0 |
| `Gobln.Pager.Json.Microsoft` | 1.0.0 | 2.0.0 |
| `Gobln.Pager.Json.Newtonsoft` | 1.0.0 | 2.0.0 |

All five packages ship as 2.0.0. The `net6.0` target framework was dropped from
`Gobln.Pager`, `.Mvc`, `.Json.Microsoft` and `.Json.Newtonsoft`, which is a
breaking change for consumers and is what the major version bump signals.

## Changes

### 1. `.github/workflows/release.yml` (new)

```yaml
name: Release

on:
  push:
    branches: [master]
  workflow_dispatch:

permissions:
  contents: write

concurrency:
  group: release-${{ github.ref }}
  cancel-in-progress: false

jobs:
  release:
    runs-on: windows-latest

    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: |
            8.0.x
            10.0.x

      - name: Restore
        run: dotnet restore Gobln.Pager.slnx

      - name: Build
        run: dotnet build Gobln.Pager.slnx -c Release --no-restore

      - name: Test
        run: dotnet test Gobln.Pager.slnx -c Release --no-build

      - name: Pack
        shell: pwsh
        run: |
          $projects = @(
            'scr/Gobln.Pager/Gobln.Pager.csproj'
            'scr/Gobln.Pager.Mvc/Gobln.Pager.Mvc.csproj'
            'scr/Gobln.Pager.Blazor/Gobln.Pager.Blazor.csproj'
            'scr/Gobln.Pager.Json.Microsoft/Gobln.Pager.Json.Microsoft.csproj'
            'scr/Gobln.Pager.Json.Newtonsoft/Gobln.Pager.Json.Newtonsoft.csproj'
          )
          foreach ($p in $projects) {
            dotnet pack $p -c Release --no-build -o artifacts
            if ($LASTEXITCODE -ne 0) { throw "pack failed: $p" }
          }

      - uses: actions/upload-artifact@v4
        with:
          name: nupkg
          path: artifacts/*.nupkg
          if-no-files-found: error

      - name: Push to NuGet
        run: >
          dotnet nuget push "artifacts/*.nupkg"
          --api-key ${{ secrets.NUGET_API_KEY }}
          --source https://api.nuget.org/v3/index.json
          --skip-duplicate

      - name: Resolve version
        id: ver
        shell: pwsh
        run: |
          $xml = [xml](Get-Content scr/Gobln.Pager/Gobln.Pager.csproj)
          $v = ($xml.Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1).Trim()
          if (-not $v) { throw "No <Version> found in Gobln.Pager.csproj" }
          "version=$v" | Out-File $env:GITHUB_OUTPUT -Append -Encoding utf8
          $tag = git ls-remote --tags origin "refs/tags/v$v"
          if ($tag) { "release=false" | Out-File $env:GITHUB_OUTPUT -Append -Encoding utf8 }
          else       { "release=true"  | Out-File $env:GITHUB_OUTPUT -Append -Encoding utf8 }

      - name: Create GitHub Release
        if: steps.ver.outputs.release == 'true'
        shell: pwsh
        env:
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        run: |
          $tag = "v${{ steps.ver.outputs.version }}"
          $files = (Get-ChildItem artifacts/*.nupkg).FullName
          gh release create $tag @files --title $tag --generate-notes
          if ($LASTEXITCODE -ne 0) { throw "gh release create failed" }
```

#### Why the pack step is a loop

GitHub wraps a multi-line `pwsh` `run:` block so that only the **last** command's
exit code is checked. Five bare `dotnet pack` lines would let failures in packages
1-4 pass unnoticed as long as package 5 succeeded. The explicit
`if ($LASTEXITCODE -ne 0) { throw }` inside the loop is what makes a failed pack
fail the job. Do not flatten it back into a list of plain commands.

### 2. `scr/Gobln.Pager.Mvc/Gobln.Pager.Mvc.csproj`

Add to the first `PropertyGroup`, following the style of `Gobln.Pager.csproj`:

- `PackageId` = `$(AssemblyName)`
- `Version` = `2.0.0`
- `Authors` = `Jorn Hendrickx`
- `Description` — one sentence on the MVC/Core HTML helper
- `PackageTags` — `Page, Pager, Paging, Pagination, Mvc`
- `PackageProjectUrl` / `RepositoryUrl` = the GitHub repo
- `PackageIcon` = `Gobln Pager.png`, with the matching `None Include` from `images/`
- `GeneratePackageOnBuild` = `true`

### 3. `scr/Gobln.Pager.Blazor/Gobln.Pager.Blazor.csproj`

Same set, with Blazor-appropriate `Description` and `PackageTags`.

### 4. Reference assemblies

Add to `Gobln.Pager`, `Gobln.Pager.Mvc`, `Gobln.Pager.Json.Microsoft` and
`Gobln.Pager.Json.Newtonsoft` (every project targeting `net462`/`net48`):

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.NETFramework.ReferenceAssemblies" Version="1.0.3" PrivateAssets="all" />
</ItemGroup>
```

`PrivateAssets="all"` keeps it out of the published package's dependency list.
This removes the build's dependency on which .NET Framework targeting packs the
`windows-latest` image happens to ship.

## How the release gate works

`--skip-duplicate` exits successfully whether or not anything was uploaded, so
its exit code cannot tell us if a release happened. Instead the workflow reads
`<Version>` from `Gobln.Pager.csproj` and asks the remote whether tag
`v<version>` exists:

- tag absent -> this version has not been released -> create tag + GitHub Release
- tag present -> nothing new -> skip the release step

`Gobln.Pager`'s version is the release marker for the whole family. Bumping only
a satellite package (for example `Gobln.Pager.Json.Newtonsoft`) publishes that
package to NuGet but creates no GitHub Release. That is intentional: the Release
tracks the core library's version line.

## Manual setup required

1. Create a push API key at nuget.org, scoped to glob `Gobln.Pager*`.
2. Add it as repository secret **`NUGET_API_KEY`**
   (Settings -> Secrets and variables -> Actions).

`GITHUB_TOKEN` is supplied automatically; `permissions: contents: write` in the
workflow grants it the rights to push the tag and create the Release.

## Known limitations

- **First run may skip a package.** All five are set to 2.0.0. Any of them
  already published at 2.0.0 on nuget.org is skipped silently by
  `--skip-duplicate`. Check the run log after the first publish and bump if
  needed.
- **No path filter on the trigger.** A README-only push runs the full pipeline
  and publishes nothing. This costs CI minutes but avoids a filter list that
  drifts out of sync with what actually affects the packages.
- **Version bumps are manual.** Forgetting to bump means the push runs green and
  publishes nothing, with no warning. The build log is the only signal.

## Verification

After merging to `master`:

1. The Actions run is green.
2. `artifacts/*.nupkg` (5 files) is downloadable from the run's artifacts.
3. The "Push to NuGet" log names each package as pushed or skipped.
4. If `Gobln.Pager`'s version is new, tag `v2.0.0` and a matching GitHub Release
   exist with the five `.nupkg` files attached.
