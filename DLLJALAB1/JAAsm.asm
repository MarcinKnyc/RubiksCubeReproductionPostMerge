ExitProcess PROTO

.data
COLORS	DB  0, 155, 72, 255, 255, 255, 183, 18, 52, 255, 213, 0, 0, 70, 173, 255, 88, 0
DISTANCERGB DD 0
DISTANCESRGB	DB  0, 0, 0, 0, 0, 0
COUNTDOWN DB 6
INDEX DD 0
warray WORD 1,2,3,4

.code
PS_2 PROC
XOR EAX, EAX
mov edi, OFFSET warray
mov INDEX, EAX

;przygotowanie wektora z kolorem piksela
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
MOV AL, byte ptr [INDEX]      
CVTSI2SS XMM4, EAX              
PSLLDQ XMM4, 4           
INC INDEX
MOV AL, byte ptr [INDEX]      
CVTSI2SS XMM4, EAX
PSLLDQ XMM4, 4
INC INDEX
MOV AL, byte ptr [INDEX]      
CVTSI2SS XMM4, EAX
inc INDEX

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
mov [DISTANCESRGB], AL
inc DISTANCESRGB

;pêtla
DEC	CL
JNZ	calcdistance

XOR EAX, EAX
mov AL, COLORS
sub COLORS, 18
mov AL, COLORS

mov AL, [RSI+1]
mov AL, [RSI+2]
mov [RSI], AL
mov [RSI+1], AL
mov AL, [COLORS]
mov AL, [COLORS+1]
mov AL, [COLORS+17]

RET;
PS_2 ENDP


END

