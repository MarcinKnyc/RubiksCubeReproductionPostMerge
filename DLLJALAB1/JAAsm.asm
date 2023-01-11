.code
FindChar_1 PROC
 MOV EAX,1 ; znaleziono znak
 CPUID
 MOV EAX, ECX
 RET ; powrot z procedury
FindChar_1 ENDP ; koniec FindChar_1

MMX_1 PROC
PADDW MM0, MM5;
RET;
MMX_1 ENDP

XMM_1 PROC
;https://students.mimuw.edu.pl/~zbyszek/asm/en/instrukcje-sse.html
  shufps xmm0,xmm0,27  ;reverse the order of element (0x1B=00 01 10 11)
RET;
XMM_1 ENDP

PS_1 PROC
subss xmm1, xmm2
RET;
PS_1 ENDP

PS_2 PROC
subps xmm1, xmm2
RET;
PS_2 ENDP


END

