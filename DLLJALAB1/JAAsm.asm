ExitProcess PROTO 

.data
DISTANCERGB DD 0
;DISTANCESRGB	DB  0, 0, 0, 0, 0, 0
;superceded by RBX
COUNTDOWN DB 6

.code
PS_2 PROC
XOR EAX, EAX
;mov AL, [RDX]
;inc RDX
;mov AL, [RDX]
;sub RDX, 1
;mov AL, [RDX]

;przygotowanie wektora z kolorem piksela
;RCX to tabela bajtów z kolorami pixeli podawana jako argument
MOV AL, byte ptr [RCX+0]      ;przesuwamy wartoœæ dla R do AL
CVTSI2SS XMM3, EAX              ;konwersja int x EAX na float
PSLLDQ XMM3, 4                ;przesuwa o DWORDa
MOV AL, byte ptr [RCX+1]      
CVTSI2SS XMM3, EAX
PSLLDQ XMM3, 4
MOV AL, byte ptr [RCX+2]      
CVTSI2SS XMM3, EAX

MOV	CL, 6
calcdistance:
;przygotowanie wektora z kolorem z kostki rubika
;RDX to tabela bajtów z kolroami pixeli podawana jako argument

XOR EAX, EAX
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
mov EAX, DISTANCERGB
mov [RBX], AL
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



mov EAX, [RBX]
mov [RSI], AL
inc RBX
inc RBX
inc RBX
inc RBX
mov AL, byte ptr [RBX]
mov [RSI+1], AL
inc RBX
inc RBX
inc RBX
inc RBX
;mov EAX, word ptr [RBX]
mov EAX, [RBX]
mov [RSI+2], AL

RET;
PS_2 ENDP


END

