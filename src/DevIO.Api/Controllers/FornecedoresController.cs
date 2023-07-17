using AutoMapper;
using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    //[ApiConventionType(typeof(DefaultApiConventions))]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository fornecedorRepository;
        private readonly IEnderecoRepository enderecoRepository;
        private readonly IFornecedorService fornecedorService;
        private readonly IMapper mapper;

        public FornecedoresController
            (IFornecedorRepository fornecedorRepository,
            IEnderecoRepository enderecoRepository,
            IFornecedorService fornecedorService,
            IMapper mapper,
            INotificador notificador) : base(notificador)
        {
            this.fornecedorRepository = fornecedorRepository;
            this.enderecoRepository = enderecoRepository;
            this.fornecedorService = fornecedorService;
            this.mapper = mapper;
        }

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
        {

            var fornecedores = mapper.Map<IEnumerable<FornecedorViewModel>>
                (await fornecedorRepository.ObterTodos());

            return Ok(fornecedores);
        }


        [HttpGet("{id:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterPorId(Guid id)
        {
            var fornecedor = mapper.Map<FornecedorViewModel>
                (await fornecedorRepository.ObterFornecedorProdutosEndereco(id));

            if (fornecedor is null) return NotFound();

            return Ok(fornecedor);
        }


        //TODO : Teste utilizando o CustomResponse
        [ClaimsAuthorize(claimName: "Fornecedor",claimValue: "Adicionar")]
        [HttpPost]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
        {
            /* if (!ModelState.IsValid) return BadRequest();

              var result = await fornecedorService.Adicionar(mapper.Map<Fornecedor>(fornecedorViewModel));

              if (!result) return BadRequest();

              return Created(Request.Host.ToString(), fornecedorViewModel);*/

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await fornecedorService.Adicionar(mapper.Map<Fornecedor>(fornecedorViewModel));

            return CustomResponse(fornecedorViewModel);

        }


        [ClaimsAuthorize(claimName: "Fornecedor", claimValue: "Atualizar")]
        [HttpPut("{id:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> Atualizar(Guid id, FornecedorViewModel fornecedorViewModel)
        {

            if (id != fornecedorViewModel.Id || !ModelState.IsValid) return BadRequest();

            var result = await fornecedorService.Atualizar(mapper.Map<Fornecedor>(fornecedorViewModel));

            if (!result) return BadRequest();

            return NoContent();
        }


        [ClaimsAuthorize(claimName: "Fornecedor", claimValue: "Excluir")]
        [HttpDelete("{id:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult> Excluir(Guid id)
        {
            return Ok();
        }


        [HttpGet("obter-endereco/{id:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<EnderecoViewModel>> ObterEnderecoPorId(Guid id)
        {
            return Ok(mapper.Map<EnderecoViewModel>(await enderecoRepository.ObterPorId(id)));
        }


        [ClaimsAuthorize(claimName: "Fornecedor", claimValue: "Atualizar")]
        [HttpGet("atualizar-endereco/{id:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult<EnderecoViewModel>> AtualizarEndereco(Guid id, EnderecoViewModel enderecoViewModel)
        {
            if (id != enderecoViewModel.Id || !ModelState.IsValid) return BadRequest();

            await enderecoRepository.Atualizar(mapper.Map<Endereco>(enderecoViewModel));

            return NoContent();
        }

    }

}


