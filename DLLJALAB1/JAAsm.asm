ExitProcess PROTO 

.data
DISTANCERGB DD 0
;DISTANCESRGB	DB  0, 0, 0, 0, 0, 0
;superceded by RBX
MAXVAL DD 195075
MINIMALDISTANCEINDEX DB 0
CURRENTDISTANCEINDEX DB 0

.code
PS_2 PROC
XOR EAX, EAX
xorps XMM3, XMM3
mov DISTANCERGB, 0
;mov AL, [RDX]
;inc RDX
;mov AL, [RDX]
;sub RDX, 1
;mov AL, [RDX]

;przygotowanie wektora z kolorem piksela
;RCX oraz RSI (2 kopie!!!) to tabela bajtów z kolorami pixeli podawana jako argument
MOV AL, byte ptr [RSI+0]      ;przesuwamy wartoœæ dla R do AL
CVTSI2SS XMM3, EAX              ;konwersja int x EAX na float
PSLLDQ XMM3, 4                ;przesuwa o DWORDa
MOV AL, byte ptr [RSI+1]      
CVTSI2SS XMM3, EAX
PSLLDQ XMM3, 4
MOV AL, byte ptr [RSI+2]      
CVTSI2SS XMM3, EAX

MOV	CL, 6
calcdistance:
;przygotowanie wektora z kolorem z kostki rubika
;RDX to tabela bajtów z kolroami pixeli podawana jako argument

XOR EAX, EAX
xorps XMM4, XMM4
mov DISTANCERGB, 0
MOV AL, byte ptr [RDX]      
CVTSI2SS XMM4, EAX              
PSLLDQ XMM4, 4           
INC RDX
MOV AL, byte ptr [RDX]      
CVTSI2SS XMM4, EAX
PSLLDQ XMM4, 4
INC RDX
MOV AL, byte ptr [RDX]      
CVTSI2SS XMM4, EAX
inc RDX

;odejmowanie i kwadrat do wyliczenia odleg³oœci
subps XMM4, XMM3
mulps XMM4, XMM4

;liczenie dystansu kwadratowego
CVTTPS2DQ XMM4, XMM4              ;konwersja na liczbê ca³kowit¹

xor EAX, EAX
PEXTRD EAX, XMM4, 0
add DISTANCERGB, EAX
xor EAX, EAX
PEXTRD EAX, XMM4, 1
add DISTANCERGB, EAX
xor EAX, EAX
PEXTRD EAX, XMM4, 2
add DISTANCERGB, EAX

;zapisanie dystansu kwadratowego
;RBX to arrayka, któr¹ dostajemy pust¹ od u¿ytkownika, do której wpisujemy dystanse
mov EAX, DISTANCERGB
mov dword ptr [RBX], EAX
mov AL, [RBX]
inc RBX
inc RBX
inc RBX
inc RBX

;pêtla
DEC	CL
JNZ	calcdistance

XOR EAX, EAX
sub RDX, 18
sub RBX, 24

mov CL, 6
mov MAXVAL, 195075
mov CURRENTDISTANCEINDEX, 0
mov MINIMALDISTANCEINDEX, 0
;znalezienie indeksu minimalnej odleg³oœci
findmin:
XOR EAX, EAX
mov EAX, dword ptr [RBX]
CMP MAXVAL, EAX
JL skipreplacingmaxvalue

mov MAXVAL, EAX
xor EAX, EAX
mov AL, CURRENTDISTANCEINDEX
mov MINIMALDISTANCEINDEX, AL

skipreplacingmaxvalue:
inc CURRENTDISTANCEINDEX
inc RBX
inc RBX
inc RBX
inc RBX

;petla
DEC	CL
JNZ	findmin



xor eax, eax
mov CL, MINIMALDISTANCEINDEX
iterateToClosestColor:
cmp CL, 0
JE foundClosestColor
INC RDX
INC RDX
INC RDX
DEC CL
JMP iterateToClosestColor

foundClosestColor:
mov AL, byte ptr [RDX]
mov [RSI], AL
INC RDX
mov AL, byte ptr [RDX]
mov [RSI+1], AL
INC RDX
mov AL, byte ptr [RDX]
mov [RSI+2], AL
INC RDX

RET;
PS_2 ENDP


END

