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

    style HTTP fill:#f9f,stroke:#333
    style AVM fill:#fb3,stroke:#333
    style AV fill:#feb,stroke:#333
    style BA fill:#dfd,stroke:#333
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