## 플레이어 이동  
### PlayerCharacter.h  
```cpp
void MoveForward(float Value);  
void MoveRight(float Value);  

float ForwardInput = 0.0f;  
float RightInput = 0.0f;
```  

### PlayerCharacter.cpp
```cpp
FVector MoveDirection =
        FVector::ForwardVector * ForwardInput +
        FVector::RightVector * RightInput;

    if (!MoveDirection.IsNearlyZero())
    {
        MoveDirection.Normalize();

PlayerInputComponent->BindAxis("MoveForward", this, &APlayerCharacter::MoveForward);
    PlayerInputComponent->BindAxis("MoveRight", this, &APlayerCharacter::MoveRight);
  
void APlayerCharacter::MoveForward(float Value)
{
    ForwardInput = Value;

    AddMovementInput(FVector::ForwardVector, Value);
}

void APlayerCharacter::MoveRight(float Value)
{
    RightInput = Value;

    AddMovementInput(FVector::RightVector, Value);
}

void APlayerCharacter::MoveForward(float Value)
{
    ForwardInput = Value;

    AddMovementInput(FVector::ForwardVector, Value);
}

void APlayerCharacter::MoveRight(float Value)
{
    RightInput = Value;

    AddMovementInput(FVector::RightVector, Value);
}
```

## 플레이어 체력  
### PlayerCharacter.h  
```cpp
 UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "HP")
    float MaxHP = 100.0f;

    UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "HP")
    float CurrentHP;

    void Die();

void TakeDamageFromEnemy(float Damage);
```

### PlayerCharacter.cpp
```cpp
CurrentHP = MaxHP;
void APlayerCharacter::TakeDamageFromEnemy(float Damage)
{
    CurrentHP -= Damage;

    UE_LOG(LogTemp, Warning, TEXT("Player HP: %f"), CurrentHP);

    if (CurrentHP <= 0.0f)
    {
        CurrentHP = 0.0f;
        Die();
    }
}

void APlayerCharacter::Die()
{
    UE_LOG(LogTemp, Warning, TEXT("Player Dead"));
}
```
