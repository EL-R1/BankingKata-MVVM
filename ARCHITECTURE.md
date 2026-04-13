# Architecture MVVM - BankingKata-MVVM

```mermaid
graph TD
    subgraph Client
        HTTP[HTTP Requests]
    end

    subgraph Controllers
        AC[AccountsController<br/>api/accounts]
        SC[SavingsController<br/>api/savings]
    end

    subgraph ViewModels
        AVM[AccountsViewModel]
        AV[AccountViewModel<br/>INotifyPropertyChanged]
        SV[SavingsAccountViewModel<br/>INotifyPropertyChanged]
        OV[OperationViewModel<br/>INotifyPropertyChanged]
        StV[StatementViewModel<br/>INotifyPropertyChanged]
    end

    subgraph Models
        BA[BankAccount]
        SA[SavingsAccount]
        T[Transaction]
    end

    subgraph Repositories
        BR[BankAccountRepository]
        SR[SavingsAccountRepository]
        TR[TransactionRepository]
    end

    HTTP --> AC
    HTTP --> SC

    AC --> AVM
    SC --> AVM

    AVM --> AV
    AVM --> SV

    AVM --> BR
    AVM --> TR
    AVM --> SR

    AV --> BA
    SV --> SA

    style HTTP fill:#e8f4f8,stroke:#333,color:#000000
    style AC fill:#d4e8f4,stroke:#333,color:#000000
    style SC fill:#d4e8f4,stroke:#333,color:#000000
    style AVM fill:#d4f4e8,stroke:#333,color:#000000
    style AV fill:#e4d4f4,stroke:#333,color:#000000
    style SV fill:#e4d4f4,stroke:#333,color:#000000
    style OV fill:#e4d4f4,stroke:#333,color:#000000
    style StV fill:#e4d4f4,stroke:#333,color:#000000
    style BA fill:#e8f4d4,stroke:#333,color:#000000
    style SA fill:#e8f4d4,stroke:#333,color:#000000
    style T fill:#e8f4d4,stroke:#333,color:#000000
    style BR fill:#d4f4e8,stroke:#333,color:#000000
    style SR fill:#d4f4e8,stroke:#333,color:#000000
    style TR fill:#d4f4e8,stroke:#333,color:#000000
```

### Pattern MVVM

- **Model** : Données métier (BankAccount, SavingsAccount, Transaction)
- **View** : Controllers API → JSON
- **ViewModel** : 
  - `AccountsViewModel` - Logique métier centralisée
  - ViewModels avec `INotifyPropertyChanged` pour binding bidirectionnel

### Caractéristiques MVVM

1. ViewModels implémentent `INotifyPropertyChanged`
2. Propriétés avec setter qui appelle `OnPropertyChanged()`
3. ViewModels gèrent la logique métier (pas les Controllers)
4. Controllers délèguent au ViewModel

### Structure du projet

```
BankingKata-MVVM/
├── Controllers/      # Endpoints API
├── ViewModels/       # Logique métier + Models avec PropertyChanged
│   ├── AccountsViewModel.cs
│   └── AccountViewModels.cs
├── Models/           # Entités
└── Repositories/     # Accès données
```