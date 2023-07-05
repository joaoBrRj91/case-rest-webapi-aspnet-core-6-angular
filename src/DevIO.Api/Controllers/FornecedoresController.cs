﻿using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository fornecedorRepository;
        private readonly IFornecedorService fornecedorService;
        private readonly IMapper mapper;

        public FornecedoresController
            (IFornecedorRepository fornecedorRepository,
            IFornecedorService fornecedorService,
            IMapper mapper)
        {
            this.fornecedorRepository = fornecedorRepository;
            this.fornecedorService = fornecedorService;
            this.mapper = mapper;
        }

        [HttpGet]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
        {

            var fornecedores = mapper.Map<IEnumerable<FornecedorViewModel>>
                (await fornecedorRepository.ObterTodos());

            return Ok(fornecedores);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterPorId(Guid id)
        {
            var fornecedor = mapper.Map<FornecedorViewModel>
                (await fornecedorRepository.ObterFornecedorProdutosEndereco(id));

            if (fornecedor is null) return NotFound();

            return Ok(fornecedor);
        }

        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await fornecedorService.Adicionar(mapper.Map<Fornecedor>(fornecedorViewModel));

            if (!result) return BadRequest();

            return Created(Request.Host.ToString(), fornecedorViewModel);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Atualizar(Guid id, FornecedorViewModel fornecedorViewModel)
        {

            if (id != fornecedorViewModel.Id || !ModelState.IsValid) return BadRequest();

            var result = await fornecedorService.Atualizar(mapper.Map<Fornecedor>(fornecedorViewModel));

            if (!result) return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Excluir(Guid id)
        {
            return Accepted();
        }

    }
}
