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

