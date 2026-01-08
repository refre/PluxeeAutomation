# PluxeeAutomation

Automatisation du processus de commande Pluxee (chèques repas électroniques) avec Playwright et .NET 9.

## Description

Ce projet automatise les étapes suivantes sur le portail Pluxee :
1. Connexion au portail client Pluxee
2. Accès à l'espace entreprises
3. Création d'une nouvelle commande
4. Sélection des chèques repas électroniques
5. Remplissage automatique du formulaire de commande
6. Validation des étapes du processus de commande

## Prérequis

- .NET 9.0 SDK
- Navigateur
- Compte Pluxee valide

## Installation

1. Clonez le repository :
```bash
git clone https://github.com/votre-username/PluxeeAutomation.git
cd PluxeeAutomation
```

2. Restaurez les packages NuGet :
```bash
dotnet restore
```

3. Installez les navigateurs Playwright :
```bash
pwsh PluxeeAutomation.PlayWright/bin/Debug/net9.0/playwright.ps1 install
```

## Configuration

Modifiez le fichier `appsettings.json` avec vos informations :

```json
{
  "PluxeeSettings": {
    "BrowserExecutablePath": BrowserPath,
    "PluxeeUrl": PluxeeURL,
    "Username": "votre-username",
    "Password": "votre-password"
  }
}
```

### Paramètres configurables

- **BrowserExecutablePath** : Chemin vers l'exécutable du navigateur Brave (ou Chromium)
- **PluxeeUrl** : URL de connexion au portail Pluxee
- **Username** : Votre identifiant Pluxee
- **Password** : Votre mot de passe Pluxee

## Utilisation

Lancez l'application :

```bash
cd PluxeeAutomation.PlayWright
dotnet run
```

L'application va :
- Ouvrir le navigateur 
- Se connecter automatiquement
- Naviguer vers la section de commande
- Remplir le formulaire avec les valeurs par défaut
- Progresser à travers les étapes de validation

## Architecture

Le projet utilise :
- **Playwright** pour l'automatisation du navigateur
- **Microsoft.Extensions.Configuration** pour la gestion de la configuration
- **Microsoft.Extensions.DependencyInjection** pour l'injection de dépendances

### Structure du projet

```
PluxeeAutomation/
├── PluxeeAutomation.PlayWright/
│   ├── Pluxee.cs              # Classe principale avec la logique d'automatisation
│   ├── PluxeeSettings.cs      # Modèle de configuration
│   ├── Program.cs             # Point d'entrée de l'application
│   └── appsettings.json       # Fichier de configuration
└── README.md
```

## Fonctionnalités

### Méthodes principales

- `InitializeAsync()` : Initialise Playwright et lance le navigateur
- `NavigateToPluxeeAsync()` : Navigation vers la page de connexion
- `LoginAsync()` : Authentification automatique
- `ClickAccesEntreprisesAsync()` : Accès à l'espace entreprises
- `ClickCreateNewOrderAsync()` : Création d'une nouvelle commande
- `ClickElectronicLunchPassAsync()` : Sélection des chèques repas électroniques
- `FillOrderFormAsync()` : Remplissage du formulaire de commande
- `ClickOrderLineNextStepAsync()` : Validation de l'étape suivante
- `CloseAsync()` : Fermeture propre du navigateur

## Personnalisation

### Modifier les valeurs de commande

Dans `Pluxee.cs`, méthode `FillOrderFormAsync()`, vous pouvez modifier :

```csharp
await _page.Locator("input[id$='orderline_units']").FillAsync("xx"); // Nombre de chèques
await _page.Locator("input[id$='orderline_faceValue']").FillAsync("xx,xx €"); // Valeur faciale
```

La date de validité est automatiquement définie au premier jour du mois suivant.

## Sécurité

⚠️ **Important** : Ne commitez jamais votre fichier `appsettings.json` avec vos identifiants réels sur un repository public.

Ajoutez `appsettings.json` à votre `.gitignore` :

```gitignore
appsettings.json
```

Créez un fichier `appsettings.example.json` comme modèle :

```json
{
  "PluxeeSettings": {
    "BrowserExecutablePath": browser,
    "PluxeeUrl": PluxeeURL,
    "Username": "your-username-here",
    "Password": "your-password-here"
  }
}
```

## Technologies utilisées

- [.NET 9](https://dotnet.microsoft.com/)
- [Playwright for .NET](https://playwright.dev/dotnet/)
- [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/)
- [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/)

## Licence

MIT

## Contributeurs

Votre nom - [@JVK](https://github.com/refre07)

## Support

Pour toute question ou problème, ouvrez une issue sur GitHub.
