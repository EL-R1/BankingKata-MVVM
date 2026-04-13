# BankingKata-MVVM

API Bancaire en .NET 8 utilisant le pattern **MVVM (Model-View-ViewModel)**.

## Architecture

```
BankingKata-MVVM/
├── Models/           # Modèles de domaine
│   └── AccountModels.cs      # BankAccount, SavingsAccount, Transaction
├── Repositories/     # Accès aux données
│   └── Repositories.cs       # In-memory repositories
├── ViewModels/       # ViewModels avec INotifyPropertyChanged
│   ├── AccountViewModels.cs   # AccountViewModel, StatementViewModel, etc.
│   └── AccountsViewModel.cs  # Logique métier centralisée
├── Controllers/      # Contrôleurs API
│   ├── AccountsController.cs
│   └── SavingsController.cs
└── Program.cs        # Point d'entrée
```

## Pattern MVVM

- **Model** : `Models/AccountModels.cs` - Données et logique métier
- **View** : Controllers API qui retournent des ViewModels en JSON
- **ViewModel** : `ViewModels/AccountsViewModel.cs` - État observable avec `INotifyPropertyChanged`

## ViewModels avec PropertyChanged

Les ViewModels implémentent `INotifyPropertyChanged` pour supporter la liaison de données bidirectionnelle :

```csharp
public class AccountViewModel : INotifyPropertyChanged
{
    private string _accountNumber;
    public string AccountNumber
    {
        get => _accountNumber;
        set { _accountNumber = value; OnPropertyChanged(); }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```

## Endpoints

### Comptes Courants
| Méthode | Endpoint | Description |
|---------|----------|-------------|
| GET | `/api/accounts` | Liste tous les comptes |
| GET | `/api/accounts/{accountNumber}` | Détails d'un compte |
| POST | `/api/accounts` | Créer un compte |
| POST | `/api/accounts/{accountNumber}/deposit` | Déposer de l'argent |
| POST | `/api/accounts/{accountNumber}/withdraw` | Retirer de l'argent |
| POST | `/api/accounts/{accountNumber}/overdraft` | Modifier le découvert |
| GET | `/api/accounts/{accountNumber}/statement` | Relevé de compte |

### Livrets Épargne
| Méthode | Endpoint | Description |
|---------|----------|-------------|
| GET | `/api/savings` | Liste tous les livrets |
| GET | `/api/savings/{accountNumber}` | Détails d'un livret |
| POST | `/api/savings` | Créer un livret |
| POST | `/api/savings/{accountNumber}/deposit` | Déposer |
| POST | `/api/savings/{accountNumber}/withdraw` | Retirer |

## Exemples de Requêtes

```bash
# Créer un compte
curl -X POST http://localhost:5000/api/accounts \
  -H "Content-Type: application/json" \
  -d '{"accountNumber": "ACC001", "initialBalance": 1000, "overdraftLimit": 500}'

# Déposer de l'argent
curl -X POST http://localhost:5000/api/accounts/ACC001/deposit \
  -H "Content-Type: application/json" \
  -d '{"amount": 500}'

# Obtenir le relevé
curl http://localhost:5000/api/accounts/ACC001/statement
```

## Lancer le Projet

```bash
cd BankingKata-MVVM
dotnet run
```

L'API sera disponible sur `http://localhost:5000`